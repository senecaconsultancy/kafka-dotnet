﻿using Kafka.Common.Model;

namespace Kafka.Client.Model
{
    public sealed record ProduceRecord(
        TopicPartition TopicPartition,
        Timestamp Timestamp,
        ReadOnlyMemory<byte>? Key,
        ReadOnlyMemory<byte>? Value,
        IReadOnlyList<RecordHeader> Headers,
        Attributes Attributes
    );
}