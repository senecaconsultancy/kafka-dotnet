﻿using Kafka.Common;
using Kafka.Common.Records;
using System.Collections.Immutable;

namespace Kafka.Client.Clients.Producer
{
    public sealed record ProducerRecord<TKey, TValue>(
        Timestamp Timestamp,
        ImmutableArray<RecordHeader> Headers,
        TKey Key,
        TValue Value
    );
}
