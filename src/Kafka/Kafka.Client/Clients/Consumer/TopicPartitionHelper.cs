﻿using Kafka.Client.Messages;
using Kafka.Common.Model;
using Kafka.Common.Model.Comparison;
using System.Collections.Immutable;

namespace Kafka.Client.Clients.Consumer
{
    internal static class TopicPartitionHelper
    {
        internal static ImmutableSortedDictionary<ClusterNodeId, ImmutableArray<TopicPartition>> GetAssignments(
            MetadataResponse metadataResponse
        ) =>
            metadataResponse
                .TopicsField
                .SelectMany(t => t.PartitionsField
                    .Select(p => (p.LeaderIdField, TopicPartition: new TopicPartition(new (t.TopicIdField, t.NameField), p.PartitionIndexField)))
                )
                .GroupBy(g => g.LeaderIdField)
                .ToImmutableSortedDictionary(
                    k => k.Key,
                    v => v.Select(r => r.TopicPartition).ToImmutableArray(),
                    ClusterNodeIdCompare.Instance
                )
            ;

        internal static void UpdateTopicPartitionOffsets(
            IDictionary<TopicPartition, Offset> topicPartitionOffsets,
            OffsetFetchResponse offsetFetchResponse
        )
        {
            // Check if stored by group. This assumes one and only one group.
            var group = offsetFetchResponse.GroupsField.FirstOrDefault();
            if (group != null)
                foreach (var topic in group.TopicsField)
                    foreach (var partition in topic.PartitionsField)
                        topicPartitionOffsets[new(topic.NameField, partition.PartitionIndexField)] = partition.CommittedOffsetField;
            // Check if stored by topic.
            foreach (var topic in offsetFetchResponse.TopicsField)
                foreach (var partition in topic.PartitionsField)
                    topicPartitionOffsets[new(topic.NameField, partition.PartitionIndexField)] = partition.CommittedOffsetField;
        }

        internal static void UpdateTopicPartitionOffsets(
            IDictionary<TopicPartition, Offset> topicPartitionOffsets,
            ListOffsetsResponse offsetListResponse
        )
        {
            foreach (var topic in offsetListResponse.TopicsField)
                foreach (var partition in topic.PartitionsField)
                    topicPartitionOffsets[new(topic.NameField, partition.PartitionIndexField)] = partition.OffsetField;
        }
    }
}
