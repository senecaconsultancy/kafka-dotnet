﻿using System.Collections.Immutable;

namespace Kafka.Common.Model
{
    public sealed record PartitionMetadata(
        TopicPartition TopicPartition,
        Error Error,
        NodeId? LeaderId,
        Timestamp? LeaderEpoch,
        ImmutableArray<NodeId> ReplicaIds,
        ImmutableArray<NodeId> InSyncReplicaIds,
        ImmutableArray<NodeId> OfflineReplicaIds
    );
}
