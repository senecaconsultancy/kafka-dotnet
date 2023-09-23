﻿using Kafka.Common.Model;
using System.Collections.Immutable;

namespace Kafka.Client.Clients.Admin.Model
{
    public sealed record ListTopicsResult(
        ImmutableArray<TopicInfo> Topics
    )
    {
        public static ListTopicsResult Empty { get; } = new(
            ImmutableArray<TopicInfo>.Empty
        );
    };
}