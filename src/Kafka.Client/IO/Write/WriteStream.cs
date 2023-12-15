﻿using Kafka.Client.Config;
using Kafka.Client.Logging;
using Kafka.Client.Messages;
using Kafka.Client.Model;
using Kafka.Client.Model.Internal;
using Kafka.Client.Net;
using Kafka.Common.Exceptions;
using Kafka.Common.Model;
using Kafka.Common.Model.Comparison;
using Kafka.Common.Net;
using Kafka.Common.Protocol;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Kafka.Client.IO.Write
{
    internal sealed class WriteStream(
        ICluster<INodeLink> connections,
        WriteStreamConfig producerConfig,
        ILogger logger
    ) :
        IWriteStream,
        IDisposable
    {
        private readonly CancellationTokenSource _internalCts = new();
        private readonly ConcurrentDictionary<NodeId, WriteChannel> _brokerChannels = [];
        private readonly ConcurrentDictionary<TopicPartition, WriteChannel> _brokerChannelsByTopicPartition = new(TopicPartitionCompare.Equality);
        private readonly ConcurrentDictionary<Topic, ProducerTopicMetadata> _producerMetadata = new(TopicCompare.Equality);
        private readonly SortedSet<TopicPartition> _transactionMembers = new(TopicPartitionCompare.Instance);
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private Attributes _attributes = Attributes.None;
        private readonly string? _transactionalId = producerConfig.TransactionalId;
        private readonly int _transactionTimeoutMs = producerConfig.TransactionTimeoutMs;
        private readonly bool _enableIdempotence = producerConfig.EnableIdempotence;
        private readonly WriteStreamConfig _producerConfig = producerConfig;
        private readonly ILogger _logger = logger;
        private readonly ICluster<INodeLink> _connections = connections;

        private bool _initialized;
        private long _producerId = -1;
        private short _producerEpoch = -1;
        private NodeId _coordinator;

        async Task<ProducerTopicMetadata> IWriteStream.MetadataForTopic(
            TopicName topic,
            CancellationToken cancellationToken
        )
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await MetadataForTopic(
                    topic,
                    cancellationToken
                ).ConfigureAwait(false);
            }
            finally
            {
                _semaphoreSlim.Release();
            }

        }

        private async Task<ProducerTopicMetadata> MetadataForTopic(
            TopicName topic,
            CancellationToken cancellationToken
        )
        {
            if (_producerMetadata.TryGetValue(topic, out var metadata) && metadata.ExpireTime < DateTimeOffset.UtcNow)
                return metadata;
            _producerMetadata.Remove(topic, out _);
            metadata = await CreateTopicMetadata(topic, cancellationToken).ConfigureAwait(false);
            _producerMetadata.TryAdd(topic, metadata);
            return metadata;
        }

        IWriterBuilder IWriteStream.CreateWriter() =>
            new WriterBuilder(
                this,
                DefaultPartitioner.Instance,
                _logger
            )
        ;

        async Task<ProduceResult> IWriteStream.Write(
            WriteRecord produceRecord,
            CancellationToken cancellationToken
        )
        {
            await EnsureProducer(cancellationToken).ConfigureAwait(false);
            await CheckTransactionMembership(
                produceRecord.TopicPartition,
                cancellationToken
            ).ConfigureAwait(false);
            var callback = new TaskCompletionSource<ProduceResult>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            var command = new ProduceCommand(
                produceRecord,
                callback
            );
            var channel = await GetChannel(
                produceRecord.TopicPartition,
                cancellationToken
            ).ConfigureAwait(false);
            channel.Send(command, cancellationToken);
            await Task.Yield();
            return await callback
                .Task
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false)
            ;
        }

        async Task<ITransaction> IWriteStream.BeginTransaction(CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (string.IsNullOrEmpty(_transactionalId))
                    throw new InvalidOperationException("Transactional Id not set");
                if (_attributes.HasFlag(Attributes.IsTransactional))
                    throw new InvalidOperationException("Transaction in progress");
                await FlushChannels(cancellationToken).ConfigureAwait(false);
                _attributes |= Attributes.IsTransactional;
                _logger.TransactionBegin();
                return new Transaction(EndTransaction);
            }
            finally { _semaphoreSlim.Release(); }
        }

        private async Task CheckTransactionMembership(
            TopicPartition topicPartition,
            CancellationToken cancellationToken
        )
        {
            if (!_attributes.HasFlag(Attributes.IsTransactional))
                return;
            if (string.IsNullOrEmpty(_transactionalId))
                throw new InvalidOperationException("Transactional Id not set");
            if (_transactionMembers.Contains(topicPartition))
                return;
            var partitions = ImmutableArray.Create(
                new AddPartitionsToTxnRequestData.AddPartitionsToTxnTopic(
                    topicPartition.Topic.TopicName,
                    [topicPartition.Partition.Value],
                    []
                )
            );
            var transactions = ImmutableArray.Create(
                new AddPartitionsToTxnRequestData.AddPartitionsToTxnTransaction(
                    _transactionalId,
                    _producerId,
                    _producerEpoch,
                    false,
                    partitions,
                    []
                )
            );
            var addPartitionsToTxnRequest = new AddPartitionsToTxnRequestData(
                TransactionsField: transactions,
                V3AndBelowTransactionalIdField: _transactionalId,
                V3AndBelowProducerIdField: _producerId,
                V3AndBelowProducerEpochField: _producerEpoch,
                V3AndBelowTopicsField: partitions,
                []
            );
            var coordinator = await _connections.Connection(
                _coordinator,
                cancellationToken
            ).ConfigureAwait(false);
            var addPartitionsToTxnResponse = await coordinator.AddPartitionsToTxn(
                addPartitionsToTxnRequest,
                cancellationToken
            ).ConfigureAwait(false);

            foreach (var topic in addPartitionsToTxnResponse.ResultsByTopicV3AndBelowField)
                foreach (var partition in topic.ResultsByPartitionField)
                    if (partition.PartitionErrorCodeField != 0)
                        throw new ApiException(ApiErrors.Translate(partition.PartitionErrorCodeField));
            _transactionMembers.Add(topicPartition);
            _logger.TransactionAdd(topicPartition);
        }

        private async Task EndTransaction(bool commit, CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (!_attributes.HasFlag(Attributes.IsTransactional))
                    throw new InvalidOperationException("No active transaction");
                if (string.IsNullOrEmpty(_transactionalId))
                    throw new InvalidOperationException("Transactional Id not set");
                await FlushChannels(cancellationToken).ConfigureAwait(false);
                var endTxnRequest = new EndTxnRequestData(
                    _transactionalId,
                    _producerId,
                    _producerEpoch,
                    commit,
                    []
                );
                var coordinator = await _connections.Connection(
                    _coordinator,
                    cancellationToken
                ).ConfigureAwait(false);
                var endTxnResponse = await coordinator.EndTxn(
                    endTxnRequest,
                    cancellationToken
                ).ConfigureAwait(false);
                if (endTxnResponse.ErrorCodeField != 0)
                    throw new ApiException(ApiErrors.Translate(endTxnResponse.ErrorCodeField));
                _transactionMembers.Clear();
                if (commit)
                    _logger.TransactionCommit();
                else
                    _logger.TransactionRollback();
                _attributes &= ~Attributes.IsTransactional;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task FlushChannels(CancellationToken cancellationToken)
        {
            var flushTasks = _brokerChannels.Select(r => r.Value.Flush(cancellationToken));
            await Task.WhenAll(flushTasks).ConfigureAwait(false);
        }

        async Task IWriteStream.Flush(CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await FlushChannels(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<WriteChannel> GetChannel(
            TopicPartition topicPartition,
            CancellationToken cancellationToken
        )
        {
            if (_brokerChannelsByTopicPartition.TryGetValue(topicPartition, out var channel))
                return channel;

            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_brokerChannelsByTopicPartition.TryGetValue(topicPartition, out channel))
                    return channel;
                var metadata = await MetadataForTopic(
                    topicPartition.Topic.TopicName,
                    cancellationToken
                ).ConfigureAwait(false);
                var nodeId = metadata.PartitionMetadata[topicPartition.Partition].LeaderId;

                if (!_brokerChannels.TryGetValue(nodeId, out channel))
                {
                    channel = await CreateChannel(
                        nodeId,
                        cancellationToken
                    ).ConfigureAwait(false);
                    _brokerChannels.TryAdd(nodeId, channel);
                }

                _brokerChannelsByTopicPartition.TryAdd(topicPartition, channel);
                return channel;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<ProducerTopicMetadata> CreateTopicMetadata(
            TopicName topic,
            CancellationToken cancellationToken
        )
        {
            var metadataRequest = new MetadataRequestData(
                ImmutableArray.Create(
                    new MetadataRequestData.MetadataRequestTopic(
                        Guid.Empty,
                        topic,
                        []
                    )
                ),
                false,
                false,
                false,
                []
            );
            var controller = await _connections.Controller(cancellationToken).ConfigureAwait(false);
            var metadataResponse = await controller.Metadata(
                metadataRequest,
                cancellationToken
            ).ConfigureAwait(false);
            var metadatacResponseTopic = metadataResponse
                .TopicsField
                .FirstOrDefault() ??
                throw new KeyNotFoundException($"Unknown topic: {topic.Value}")
            ;
            var partitionsBuilder = ImmutableArray.CreateBuilder<ProducerPartitionMetadata>();
            foreach (var partition in metadatacResponseTopic.PartitionsField.OrderBy(r => r.PartitionIndexField))
            {
                var broker = metadataResponse
                    .BrokersField
                    .First(r => r.NodeIdField == partition.LeaderIdField)
                ;
                var producerPartitionMetadata = new ProducerPartitionMetadata(
                    partition.PartitionIndexField,
                    broker.NodeIdField,
                    broker.HostField,
                    broker.PortField
                );
                partitionsBuilder.Add(producerPartitionMetadata);
            }
            return new ProducerTopicMetadata(
                topic,
                partitionsBuilder.ToImmutable(),
                DateTimeOffset.UtcNow.AddSeconds(10)
            );
        }

        private async Task<WriteChannel> CreateChannel(
            NodeId nodeId,
            CancellationToken cancellationToken
        )
        {
            var connection = await _connections.Connection(nodeId, cancellationToken)
                .ConfigureAwait(false)
            ;
            return new(
                nodeId,
                _producerId,
                _producerEpoch,
                connection,
                _producerConfig,
                _logger
            );
        }

        private async Task EnsureProducer(
            CancellationToken cancellationToken
        )
        {
            if (_initialized)
                return;

            await _semaphoreSlim.WaitAsync(
                cancellationToken
            ).ConfigureAwait(false);
            try
            {
                if (_initialized)
                    return;

                if (string.IsNullOrEmpty(_transactionalId) && !_enableIdempotence)
                {
                    _initialized = true;
                    _logger.WriteInstance(_producerId, _producerEpoch, _enableIdempotence, _transactionalId);
                    return;
                }

                var coordinator = await _connections.Controller(
                    cancellationToken
                ).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(_transactionalId))
                {
                    _coordinator = await FindCoordinator(
                        coordinator,
                        _transactionalId,
                        cancellationToken
                    ).ConfigureAwait(false);
                    _logger.TransactionCoordinator(_coordinator);
                }

                var initProducerIdRequest = new InitProducerIdRequestData(
                    _transactionalId,
                    _transactionTimeoutMs,
                    _producerId,
                    _producerEpoch,
                    []
                );
                var initProducerIdResponse = await coordinator.InitProducerId(
                    initProducerIdRequest,
                    cancellationToken
                ).ConfigureAwait(false);

                _producerId = initProducerIdResponse.ProducerIdField;
                _producerEpoch = initProducerIdResponse.ProducerEpochField;
                _logger.WriteInstance(_producerId, _producerEpoch, _enableIdempotence, _transactionalId);
                _initialized = true;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private static async Task<NodeId> FindCoordinator(
            INodeLink protocol,
            string transactionalId,
            CancellationToken cancellationToken
        )
        {
            var findCoordinatorRequest = new FindCoordinatorRequestData(
                transactionalId,
                (sbyte)CoordinatorType.TRANSACTION,
                [transactionalId],
                []
            );
            var findCoordinatorResponse = await protocol.FindCoordinator(
                findCoordinatorRequest,
                cancellationToken
            ).ConfigureAwait(false);
            var nodeId = findCoordinatorResponse.NodeIdField;
            if (findCoordinatorResponse.CoordinatorsField.Any())
                nodeId = findCoordinatorResponse.CoordinatorsField[0].NodeIdField;
            return nodeId;
        }

        async Task IWriteStream.Close(CancellationToken cancellationToken)
        {
            await _internalCts.CancelAsync().ConfigureAwait(false);
            var channelClose = _brokerChannels
                .Select(r => r.Value.Close(cancellationToken))
            ;
            await Task.WhenAll(channelClose).ConfigureAwait(false);
        }

        void IDisposable.Dispose()
        {
            _internalCts.Dispose();
            _semaphoreSlim.Dispose();
        }
    }
}