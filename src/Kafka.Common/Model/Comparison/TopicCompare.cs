﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Kafka.Common.Model.Comparison
{
    public sealed class TopicCompare :
        IComparer<Topic>,
        IEqualityComparer<Topic>
    {
        private static readonly TopicCompare INSTANCE = new();
        private TopicCompare() { }
        public static IComparer<Topic> Instance => INSTANCE;
        public static IEqualityComparer<Topic> Equality => INSTANCE;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IComparer<Topic>.Compare(Topic x, Topic y) =>
            TopicNameCompare.Instance.Compare(x.TopicName, y.TopicName)
        ;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Topic x, Topic y) =>
            TopicNameCompare.Equality.Equals(x.TopicName, y.TopicName)
        ;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode([DisallowNull] Topic obj) =>
            TopicNameCompare.Equality.GetHashCode(obj.TopicName)
        ;
    }
}
