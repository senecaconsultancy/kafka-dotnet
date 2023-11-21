﻿using System.Runtime.CompilerServices;

namespace Kafka.Common.Model.Comparison
{
    public sealed class TopicPartitionCompare :
        IComparer<TopicPartition>,
        IEqualityComparer<TopicPartition>
    {
        private static readonly TopicPartitionCompare INSTANCE = new();
        private TopicPartitionCompare() { }
        public static IComparer<TopicPartition> Instance => INSTANCE;
        public static IEqualityComparer<TopicPartition> Equality => INSTANCE;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IComparer<TopicPartition>.Compare(TopicPartition x, TopicPartition y) =>
            TopicCompare.Instance.Compare(x.Topic, y.Topic) switch
            {
                0 => x.Partition.Value.CompareTo(y.Partition.Value),
                int v => v
            }
        ;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IEqualityComparer<TopicPartition>.Equals(TopicPartition x, TopicPartition y) =>
            TopicCompare.Equality.Equals(x.Topic, y.Topic) && x.Partition == y.Partition
        ;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IEqualityComparer<TopicPartition>.GetHashCode(TopicPartition obj) =>
            HashCode.Combine(TopicCompare.Equality.GetHashCode(obj.Topic), obj.Partition.Value)
        ;
    }
}
