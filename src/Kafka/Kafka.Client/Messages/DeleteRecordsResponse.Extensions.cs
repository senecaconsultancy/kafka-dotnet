using System.CodeDom.Compiler;
using Kafka.Common.Encoding;
using DeleteRecordsPartitionResult = Kafka.Client.Messages.DeleteRecordsResponse.DeleteRecordsTopicResult.DeleteRecordsPartitionResult;
using DeleteRecordsTopicResult = Kafka.Client.Messages.DeleteRecordsResponse.DeleteRecordsTopicResult;

namespace Kafka.Client.Messages
{
    [GeneratedCode("kgen", "1.0.0.0")]
    public static class DeleteRecordsResponseSerde
    {
        private static readonly DecodeDelegate<DeleteRecordsResponse>[] READ_VERSIONS = {
            ReadV00,
            ReadV01,
            ReadV02,
        };
        private static readonly EncodeDelegate<DeleteRecordsResponse>[] WRITE_VERSIONS = {
            WriteV00,
            WriteV01,
            WriteV02,
        };
        public static DeleteRecordsResponse Read(byte[] buffer, ref int index, short version) =>
            READ_VERSIONS[version](buffer, ref index)
        ;
        public static int Write(byte[] buffer, int index, DeleteRecordsResponse message, short version) =>
            WRITE_VERSIONS[version](buffer, index, message)
        ;
        private static DeleteRecordsResponse ReadV00(byte[] buffer, ref int index)
        {
            var throttleTimeMsField = Decoder.ReadInt32(buffer, ref index);
            var topicsField = Decoder.ReadArray<DeleteRecordsTopicResult>(buffer, ref index, DeleteRecordsTopicResultSerde.ReadV00) ?? throw new NullReferenceException("Null not allowed for 'Topics'");
            return new(
                throttleTimeMsField,
                topicsField
            );
        }
        private static int WriteV00(byte[] buffer, int index, DeleteRecordsResponse message)
        {
            index = Encoder.WriteInt32(buffer, index, message.ThrottleTimeMsField);
            index = Encoder.WriteArray<DeleteRecordsTopicResult>(buffer, index, message.TopicsField, DeleteRecordsTopicResultSerde.WriteV00);
            return index;
        }
        private static DeleteRecordsResponse ReadV01(byte[] buffer, ref int index)
        {
            var throttleTimeMsField = Decoder.ReadInt32(buffer, ref index);
            var topicsField = Decoder.ReadArray<DeleteRecordsTopicResult>(buffer, ref index, DeleteRecordsTopicResultSerde.ReadV01) ?? throw new NullReferenceException("Null not allowed for 'Topics'");
            return new(
                throttleTimeMsField,
                topicsField
            );
        }
        private static int WriteV01(byte[] buffer, int index, DeleteRecordsResponse message)
        {
            index = Encoder.WriteInt32(buffer, index, message.ThrottleTimeMsField);
            index = Encoder.WriteArray<DeleteRecordsTopicResult>(buffer, index, message.TopicsField, DeleteRecordsTopicResultSerde.WriteV01);
            return index;
        }
        private static DeleteRecordsResponse ReadV02(byte[] buffer, ref int index)
        {
            var throttleTimeMsField = Decoder.ReadInt32(buffer, ref index);
            var topicsField = Decoder.ReadCompactArray<DeleteRecordsTopicResult>(buffer, ref index, DeleteRecordsTopicResultSerde.ReadV02) ?? throw new NullReferenceException("Null not allowed for 'Topics'");
            _ = Decoder.ReadVarUInt32(buffer, ref index);
            return new(
                throttleTimeMsField,
                topicsField
            );
        }
        private static int WriteV02(byte[] buffer, int index, DeleteRecordsResponse message)
        {
            index = Encoder.WriteInt32(buffer, index, message.ThrottleTimeMsField);
            index = Encoder.WriteCompactArray<DeleteRecordsTopicResult>(buffer, index, message.TopicsField, DeleteRecordsTopicResultSerde.WriteV02);
            index = Encoder.WriteVarUInt32(buffer, index, 0);
            return index;
        }
        private static class DeleteRecordsTopicResultSerde
        {
            public static DeleteRecordsTopicResult ReadV00(byte[] buffer, ref int index)
            {
                var nameField = Decoder.ReadString(buffer, ref index);
                var partitionsField = Decoder.ReadArray<DeleteRecordsPartitionResult>(buffer, ref index, DeleteRecordsPartitionResultSerde.ReadV00) ?? throw new NullReferenceException("Null not allowed for 'Partitions'");
                return new(
                    nameField,
                    partitionsField
                );
            }
            public static int WriteV00(byte[] buffer, int index, DeleteRecordsTopicResult message)
            {
                index = Encoder.WriteString(buffer, index, message.NameField);
                index = Encoder.WriteArray<DeleteRecordsPartitionResult>(buffer, index, message.PartitionsField, DeleteRecordsPartitionResultSerde.WriteV00);
                return index;
            }
            public static DeleteRecordsTopicResult ReadV01(byte[] buffer, ref int index)
            {
                var nameField = Decoder.ReadString(buffer, ref index);
                var partitionsField = Decoder.ReadArray<DeleteRecordsPartitionResult>(buffer, ref index, DeleteRecordsPartitionResultSerde.ReadV01) ?? throw new NullReferenceException("Null not allowed for 'Partitions'");
                return new(
                    nameField,
                    partitionsField
                );
            }
            public static int WriteV01(byte[] buffer, int index, DeleteRecordsTopicResult message)
            {
                index = Encoder.WriteString(buffer, index, message.NameField);
                index = Encoder.WriteArray<DeleteRecordsPartitionResult>(buffer, index, message.PartitionsField, DeleteRecordsPartitionResultSerde.WriteV01);
                return index;
            }
            public static DeleteRecordsTopicResult ReadV02(byte[] buffer, ref int index)
            {
                var nameField = Decoder.ReadCompactString(buffer, ref index);
                var partitionsField = Decoder.ReadCompactArray<DeleteRecordsPartitionResult>(buffer, ref index, DeleteRecordsPartitionResultSerde.ReadV02) ?? throw new NullReferenceException("Null not allowed for 'Partitions'");
                _ = Decoder.ReadVarUInt32(buffer, ref index);
                return new(
                    nameField,
                    partitionsField
                );
            }
            public static int WriteV02(byte[] buffer, int index, DeleteRecordsTopicResult message)
            {
                index = Encoder.WriteCompactString(buffer, index, message.NameField);
                index = Encoder.WriteCompactArray<DeleteRecordsPartitionResult>(buffer, index, message.PartitionsField, DeleteRecordsPartitionResultSerde.WriteV02);
                index = Encoder.WriteVarUInt32(buffer, index, 0);
                return index;
            }
            private static class DeleteRecordsPartitionResultSerde
            {
                public static DeleteRecordsPartitionResult ReadV00(byte[] buffer, ref int index)
                {
                    var partitionIndexField = Decoder.ReadInt32(buffer, ref index);
                    var lowWatermarkField = Decoder.ReadInt64(buffer, ref index);
                    var errorCodeField = Decoder.ReadInt16(buffer, ref index);
                    return new(
                        partitionIndexField,
                        lowWatermarkField,
                        errorCodeField
                    );
                }
                public static int WriteV00(byte[] buffer, int index, DeleteRecordsPartitionResult message)
                {
                    index = Encoder.WriteInt32(buffer, index, message.PartitionIndexField);
                    index = Encoder.WriteInt64(buffer, index, message.LowWatermarkField);
                    index = Encoder.WriteInt16(buffer, index, message.ErrorCodeField);
                    return index;
                }
                public static DeleteRecordsPartitionResult ReadV01(byte[] buffer, ref int index)
                {
                    var partitionIndexField = Decoder.ReadInt32(buffer, ref index);
                    var lowWatermarkField = Decoder.ReadInt64(buffer, ref index);
                    var errorCodeField = Decoder.ReadInt16(buffer, ref index);
                    return new(
                        partitionIndexField,
                        lowWatermarkField,
                        errorCodeField
                    );
                }
                public static int WriteV01(byte[] buffer, int index, DeleteRecordsPartitionResult message)
                {
                    index = Encoder.WriteInt32(buffer, index, message.PartitionIndexField);
                    index = Encoder.WriteInt64(buffer, index, message.LowWatermarkField);
                    index = Encoder.WriteInt16(buffer, index, message.ErrorCodeField);
                    return index;
                }
                public static DeleteRecordsPartitionResult ReadV02(byte[] buffer, ref int index)
                {
                    var partitionIndexField = Decoder.ReadInt32(buffer, ref index);
                    var lowWatermarkField = Decoder.ReadInt64(buffer, ref index);
                    var errorCodeField = Decoder.ReadInt16(buffer, ref index);
                    _ = Decoder.ReadVarUInt32(buffer, ref index);
                    return new(
                        partitionIndexField,
                        lowWatermarkField,
                        errorCodeField
                    );
                }
                public static int WriteV02(byte[] buffer, int index, DeleteRecordsPartitionResult message)
                {
                    index = Encoder.WriteInt32(buffer, index, message.PartitionIndexField);
                    index = Encoder.WriteInt64(buffer, index, message.LowWatermarkField);
                    index = Encoder.WriteInt16(buffer, index, message.ErrorCodeField);
                    index = Encoder.WriteVarUInt32(buffer, index, 0);
                    return index;
                }
            }
        }
    }
}