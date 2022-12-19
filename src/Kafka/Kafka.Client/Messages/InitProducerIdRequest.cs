using System.CodeDom.Compiler;
using Kafka.Common.Protocol;

namespace Kafka.Client.Messages
{
    /// <summary>
    /// <param name="TransactionalIdField">The transactional id, or null if the producer is not transactional.</param>
    /// <param name="TransactionTimeoutMsField">The time in ms to wait before aborting idle transactions sent by this producer. This is only relevant if a TransactionalId has been defined.</param>
    /// <param name="ProducerIdField">The producer id. This is used to disambiguate requests if a transactional id is reused following its expiration.</param>
    /// <param name="ProducerEpochField">The producer's current epoch. This will be checked against the producer epoch on the broker, and the request will return an error if they do not match.</param>
    /// </summary>
    [GeneratedCode("kgen", "1.0.0.0")]
    public sealed record InitProducerIdRequest (
        string? TransactionalIdField,
        int TransactionTimeoutMsField,
        long ProducerIdField,
        short ProducerEpochField
    ) : Request(22)
    {
        public static InitProducerIdRequest Empty { get; } = new(
            default(string?),
            default(int),
            default(long),
            default(short)
        );
        public static short FlexibleVersion { get; } = 2;
    };
}