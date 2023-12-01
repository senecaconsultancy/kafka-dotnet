﻿namespace Kafka.Common.Model
{
    public readonly record struct Topic(
        TopicId TopicId,
        TopicName TopicName
    )
    {
        public static readonly Topic Empty = new(TopicId.Empty, TopicName.Empty);
        public static implicit operator Topic(Guid id) => new(id, TopicName.Empty);
        public static implicit operator Topic(TopicName topicName) => new(topicName.Value, topicName);
        public static implicit operator Topic(string? topicName) => new(TopicId.Empty, topicName);
        public static implicit operator Topic((Guid Id, string? TopicName) v) => new(v.Id, v.TopicName);
    }
}
