﻿using Kafka.Client.Model;
using Kafka.Common.Encoding;
using Kafka.Common.Model;
using Kafka.Common.Records;
using System.Collections;

namespace Kafka.Client.IO.Stream
{
    internal sealed class ProduceRecords :
        IRecords,
        IEnumerable<ProduceCommand>
    {
        private readonly int _partitionLeaderEpoch;
        private readonly Attributes _attributes;
        private readonly long _baseTimestamp;
        private readonly long _producerId;
        private readonly short _producerEpoch;
        private readonly List<IRecord> _packedRecords = new();

        private int _baseSequence;
        private int _batchSize;
        private long _maxTimestamp;

        private sealed record PackedRecord(
            int Length,
            long TimestampDelta,
            int OffsetDelta,
            ProduceCommand ProduceCommand
        ) : IRecord
        {
            int IRecord.Length => Length;

            Attributes IRecord.Attributes => Attributes.None;

            long IRecord.TimestampDelta => TimestampDelta;

            int IRecord.OffsetDelta => OffsetDelta;

            ReadOnlyMemory<byte>? IRecord.Key => ProduceCommand.Record.Key;

            ReadOnlyMemory<byte>? IRecord.Value => ProduceCommand.Record.Value;

            IReadOnlyList<RecordHeader> IRecord.Headers => ProduceCommand.Record.Headers;
        }

        public ProduceRecords(
            int partitionLeaderEpoch,
            Attributes attributes,
            long baseTimestamp,
            long producerId,
            short producerEpoch
        )
        {
            _partitionLeaderEpoch = partitionLeaderEpoch;
            _attributes = attributes;
            _baseTimestamp = baseTimestamp;
            _producerId = producerId;
            _producerEpoch = producerEpoch;
        }

        public int BatchSize => _batchSize;

        long IRecords.BaseOffset => 0;

        int IRecords.BatchLength => _batchSize;

        int IRecords.PartitionLeaderEpoch => _partitionLeaderEpoch;

        sbyte IRecords.Magic => 2;

        int IRecords.Crc => 0;

        Attributes IRecords.Attributes => _attributes;

        int IRecords.LastOffsetDelta => _packedRecords.Count - 1;

        long IRecords.BaseTimestamp => _baseTimestamp;

        long IRecords.MaxTimestamp => _maxTimestamp;

        long IRecords.ProducerId => _producerId;

        short IRecords.ProducerEpoch => _producerEpoch;

        int IRecords.BaseSequence => _baseSequence;

        IReadOnlyList<IRecord> IRecords.Records => _packedRecords;

        /// <summary>
        /// Tries to add a record to the collection.
        /// Will always add at least one record and subsequent additions are subject to the max size limit provided.
        /// </summary>
        /// <param name="sendCommand">The send command to add.</param>
        /// <param name="maxSize">The size limit to add.</param>
        /// <returns>A tuple containing: A boolean indicating if the record was added and, The total number of bytes required.</returns>
        public AddRecordResult TryAdd(
            in ProduceCommand produceCommand,
            in int maxSize
        )
        {
            var (record, _) = produceCommand;
            var timestampDelta = produceCommand.Record.Timestamp.TimestampMs - _baseTimestamp;
            var offsetDelta = _packedRecords.Count;
            var recordSize = BinaryEncoder.ComputeRecordSize(
                timestampDelta,
                offsetDelta,
                record.Key,
                record.Value,
                record.Headers
            );
            var bufferSize = BinaryEncoder.SizeOfVarInt32(recordSize) + recordSize;
            if (_packedRecords.Count > 0 && bufferSize > maxSize)
                return (false, bufferSize);
            var packedRecord = new PackedRecord(
                recordSize,
                timestampDelta,
                offsetDelta,
                produceCommand
            );
            _packedRecords.Add(packedRecord);
            _maxTimestamp = Math.Max(_maxTimestamp, record.Timestamp.TimestampMs);
            _batchSize += bufferSize;
            return (true, bufferSize);
        }

        public void SetBaseSequence(int baseSequence) =>
            _baseSequence = baseSequence
        ;

        public IEnumerator<ProduceCommand> GetEnumerator()
        {
            for (int i = 0; i < _packedRecords.Count; i++)
                yield return ((PackedRecord)_packedRecords[i]).ProduceCommand;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator()
        ;
    }
}