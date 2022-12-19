using System.CodeDom.Compiler;
using Kafka.Common.Encoding;
using TopicData = Kafka.Client.Messages.BeginQuorumEpochResponse.TopicData;
using PartitionData = Kafka.Client.Messages.BeginQuorumEpochResponse.TopicData.PartitionData;

namespace Kafka.Client.Messages
{
    [GeneratedCode("kgen", "1.0.0.0")]
    public static class BeginQuorumEpochResponseSerde
    {
        private static readonly DecodeDelegate<BeginQuorumEpochResponse>[] READ_VERSIONS = {
            ReadV00,
        };
        private static readonly EncodeDelegate<BeginQuorumEpochResponse>[] WRITE_VERSIONS = {
            WriteV00,
        };
        public static BeginQuorumEpochResponse Read(byte[] buffer, ref int index, short version) =>
            READ_VERSIONS[version](buffer, ref index)
        ;
        public static int Write(byte[] buffer, int index, BeginQuorumEpochResponse message, short version) =>
            WRITE_VERSIONS[version](buffer, index, message)
        ;
        private static BeginQuorumEpochResponse ReadV00(byte[] buffer, ref int index)
        {
            var errorCodeField = Decoder.ReadInt16(buffer, ref index);
            var topicsField = Decoder.ReadArray<TopicData>(buffer, ref index, TopicDataSerde.ReadV00) ?? throw new NullReferenceException("Null not allowed for 'Topics'");
            return new(
                errorCodeField,
                topicsField
            );
        }
        private static int WriteV00(byte[] buffer, int index, BeginQuorumEpochResponse message)
        {
            index = Encoder.WriteInt16(buffer, index, message.ErrorCodeField);
            index = Encoder.WriteArray<TopicData>(buffer, index, message.TopicsField, TopicDataSerde.WriteV00);
            return index;
        }
        private static class TopicDataSerde
        {
            public static TopicData ReadV00(byte[] buffer, ref int index)
            {
                var topicNameField = Decoder.ReadString(buffer, ref index);
                var partitionsField = Decoder.ReadArray<PartitionData>(buffer, ref index, PartitionDataSerde.ReadV00) ?? throw new NullReferenceException("Null not allowed for 'Partitions'");
                return new(
                    topicNameField,
                    partitionsField
                );
            }
            public static int WriteV00(byte[] buffer, int index, TopicData message)
            {
                index = Encoder.WriteString(buffer, index, message.TopicNameField);
                index = Encoder.WriteArray<PartitionData>(buffer, index, message.PartitionsField, PartitionDataSerde.WriteV00);
                return index;
            }
            private static class PartitionDataSerde
            {
                public static PartitionData ReadV00(byte[] buffer, ref int index)
                {
                    var partitionIndexField = Decoder.ReadInt32(buffer, ref index);
                    var errorCodeField = Decoder.ReadInt16(buffer, ref index);
                    var leaderIdField = Decoder.ReadInt32(buffer, ref index);
                    var leaderEpochField = Decoder.ReadInt32(buffer, ref index);
                    return new(
                        partitionIndexField,
                        errorCodeField,
                        leaderIdField,
                        leaderEpochField
                    );
                }
                public static int WriteV00(byte[] buffer, int index, PartitionData message)
                {
                    index = Encoder.WriteInt32(buffer, index, message.PartitionIndexField);
                    index = Encoder.WriteInt16(buffer, index, message.ErrorCodeField);
                    index = Encoder.WriteInt32(buffer, index, message.LeaderIdField);
                    index = Encoder.WriteInt32(buffer, index, message.LeaderEpochField);
                    return index;
                }
            }
        }
    }
}