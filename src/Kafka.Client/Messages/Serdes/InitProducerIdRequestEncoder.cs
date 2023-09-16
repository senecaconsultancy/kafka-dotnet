using Kafka.Common.Encoding;
using Kafka.Common.Model;
using Kafka.Common.Model.Extensions;
using Kafka.Common.Protocol;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Kafka.Client.Messages.Serdes
{
    [GeneratedCodeAttribute("kgen", "1.0.0.0")]
    public class InitProducerIdRequestEncoder : 
        RequestEncoder<RequestHeaderData, InitProducerIdRequestData>
    {
        public InitProducerIdRequestEncoder() :
            base(
                ApiKey.InitProducerId,
                new(0, 4),
                new(2, 32767),
                RequestHeaderEncoder.WriteV0,
                WriteV0
            )
        { }
        protected override EncodeDelegate<RequestHeaderData> GetHeaderEncoder(short apiVersion)
        {
            if (_flexibleVersions.Includes(apiVersion))
                return RequestHeaderEncoder.WriteV2;
            else
                return RequestHeaderEncoder.WriteV1;
        }
        protected override EncodeDelegate<InitProducerIdRequestData> GetMessageEncoder(short apiVersion) =>
            apiVersion switch
            {
                0 => WriteV0,
                1 => WriteV1,
                2 => WriteV2,
                3 => WriteV3,
                4 => WriteV4,
                _ => throw new NotSupportedException()
            }
        ;
        private static int WriteV0(byte[] buffer, int index, InitProducerIdRequestData message)
        {
            index = BinaryEncoder.WriteNullableString(buffer, index, message.TransactionalIdField);
            index = BinaryEncoder.WriteInt32(buffer, index, message.TransactionTimeoutMsField);
            return index;
        }
        private static int WriteV1(byte[] buffer, int index, InitProducerIdRequestData message)
        {
            index = BinaryEncoder.WriteNullableString(buffer, index, message.TransactionalIdField);
            index = BinaryEncoder.WriteInt32(buffer, index, message.TransactionTimeoutMsField);
            return index;
        }
        private static int WriteV2(byte[] buffer, int index, InitProducerIdRequestData message)
        {
            index = BinaryEncoder.WriteCompactNullableString(buffer, index, message.TransactionalIdField);
            index = BinaryEncoder.WriteInt32(buffer, index, message.TransactionTimeoutMsField);
            var taggedFieldsCount = 0u;
            var previousTagged = -1;
            taggedFieldsCount += (uint)message.TaggedFields.Length;
            index = BinaryEncoder.WriteVarUInt32(buffer, index, taggedFieldsCount);
            foreach(var taggedField in message.TaggedFields)
            {
                if(taggedField.Tag <= previousTagged)
                    throw new InvalidOperationException($"Reserved or out of order tag: {taggedField.Tag} - Reserved Range: -1");
                index = BinaryEncoder.WriteVarInt32(buffer, index, taggedField.Tag);
                index = BinaryEncoder.WriteCompactBytes(buffer, index, taggedField.Value);
            }
            return index;
        }
        private static int WriteV3(byte[] buffer, int index, InitProducerIdRequestData message)
        {
            index = BinaryEncoder.WriteCompactNullableString(buffer, index, message.TransactionalIdField);
            index = BinaryEncoder.WriteInt32(buffer, index, message.TransactionTimeoutMsField);
            index = BinaryEncoder.WriteInt64(buffer, index, message.ProducerIdField);
            index = BinaryEncoder.WriteInt16(buffer, index, message.ProducerEpochField);
            var taggedFieldsCount = 0u;
            var previousTagged = -1;
            taggedFieldsCount += (uint)message.TaggedFields.Length;
            index = BinaryEncoder.WriteVarUInt32(buffer, index, taggedFieldsCount);
            foreach(var taggedField in message.TaggedFields)
            {
                if(taggedField.Tag <= previousTagged)
                    throw new InvalidOperationException($"Reserved or out of order tag: {taggedField.Tag} - Reserved Range: -1");
                index = BinaryEncoder.WriteVarInt32(buffer, index, taggedField.Tag);
                index = BinaryEncoder.WriteCompactBytes(buffer, index, taggedField.Value);
            }
            return index;
        }
        private static int WriteV4(byte[] buffer, int index, InitProducerIdRequestData message)
        {
            index = BinaryEncoder.WriteCompactNullableString(buffer, index, message.TransactionalIdField);
            index = BinaryEncoder.WriteInt32(buffer, index, message.TransactionTimeoutMsField);
            index = BinaryEncoder.WriteInt64(buffer, index, message.ProducerIdField);
            index = BinaryEncoder.WriteInt16(buffer, index, message.ProducerEpochField);
            var taggedFieldsCount = 0u;
            var previousTagged = -1;
            taggedFieldsCount += (uint)message.TaggedFields.Length;
            index = BinaryEncoder.WriteVarUInt32(buffer, index, taggedFieldsCount);
            foreach(var taggedField in message.TaggedFields)
            {
                if(taggedField.Tag <= previousTagged)
                    throw new InvalidOperationException($"Reserved or out of order tag: {taggedField.Tag} - Reserved Range: -1");
                index = BinaryEncoder.WriteVarInt32(buffer, index, taggedField.Tag);
                index = BinaryEncoder.WriteCompactBytes(buffer, index, taggedField.Value);
            }
            return index;
        }
    }
}
