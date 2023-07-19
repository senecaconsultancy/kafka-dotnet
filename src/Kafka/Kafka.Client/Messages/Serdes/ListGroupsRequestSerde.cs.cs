using Kafka.Common.Encoding;
using Kafka.Common.Exceptions;
using Kafka.Common.Model;
using Kafka.Common.Model.Extensions;
using Kafka.Common.Protocol;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Version = Kafka.Common.Model.Version;

namespace Kafka.Client.Messages.Serdes
{
    [GeneratedCode("kgen", "1.0.0.0")]
    public static class ListGroupsRequestSerde
    {
        private static readonly ApiKey API_KEY = new(16);
        private static readonly VersionRange API_VERSIONS = new(0, 4);
        private static readonly VersionRange FLEXBILE_VERSIONS = new (3, 32767);
        public static IEncoder<RequestHeader, ListGroupsRequest> CreateEncoder(Version apiVersion)
        {
            apiVersion = apiVersion <= 4 ? apiVersion : new Version(4);
            var flexible = FLEXBILE_VERSIONS.Includes(apiVersion);
            var headerEncoder = RequestHeaderSerde.CreateEncoder(flexible);
            switch (apiVersion)
            {
                case 0:
                    return new Encoder<RequestHeader, ListGroupsRequest>(API_KEY, 0, flexible, headerEncoder, WriteV0);
                case 1:
                    return new Encoder<RequestHeader, ListGroupsRequest>(API_KEY, 1, flexible, headerEncoder, WriteV1);
                case 2:
                    return new Encoder<RequestHeader, ListGroupsRequest>(API_KEY, 2, flexible, headerEncoder, WriteV2);
                case 3:
                    return new Encoder<RequestHeader, ListGroupsRequest>(API_KEY, 3, flexible, headerEncoder, WriteV3);
                case 4:
                    return new Encoder<RequestHeader, ListGroupsRequest>(API_KEY, 4, flexible, headerEncoder, WriteV4);
                default:
                    throw new UnsupportedVersionException();
            }
        }
        public static IDecoder<RequestHeader, ListGroupsRequest> CreateDecoder(Version apiVersion)
        {
            apiVersion = apiVersion <= 4 ? apiVersion : new Version(4);
            var flexible = FLEXBILE_VERSIONS.Includes(apiVersion);
            var headerDecoder = RequestHeaderSerde.CreateDecoder(flexible);
            switch (apiVersion)
            {
                case 0:
                    return new Decoder<RequestHeader, ListGroupsRequest>(API_KEY, 0, flexible, headerDecoder, ReadV0);
                case 1:
                    return new Decoder<RequestHeader, ListGroupsRequest>(API_KEY, 1, flexible, headerDecoder, ReadV1);
                case 2:
                    return new Decoder<RequestHeader, ListGroupsRequest>(API_KEY, 2, flexible, headerDecoder, ReadV2);
                case 3:
                    return new Decoder<RequestHeader, ListGroupsRequest>(API_KEY, 3, flexible, headerDecoder, ReadV3);
                case 4:
                    return new Decoder<RequestHeader, ListGroupsRequest>(API_KEY, 4, flexible, headerDecoder, ReadV4);
                default:
                    throw new UnsupportedVersionException();
            }
        }
        private static int WriteV0(byte[] buffer, int index, ListGroupsRequest message)
        {
            return index;
        }
        private static (int Offset, ListGroupsRequest Value) ReadV0(byte[] buffer, int index)
        {
            var statesFilterField = ImmutableArray<string>.Empty;
            var taggedFields = ImmutableArray<TaggedField>.Empty;
            return (index, new(
                statesFilterField,
                taggedFields
            ));
        }
        private static int WriteV1(byte[] buffer, int index, ListGroupsRequest message)
        {
            return index;
        }
        private static (int Offset, ListGroupsRequest Value) ReadV1(byte[] buffer, int index)
        {
            var statesFilterField = ImmutableArray<string>.Empty;
            var taggedFields = ImmutableArray<TaggedField>.Empty;
            return (index, new(
                statesFilterField,
                taggedFields
            ));
        }
        private static int WriteV2(byte[] buffer, int index, ListGroupsRequest message)
        {
            return index;
        }
        private static (int Offset, ListGroupsRequest Value) ReadV2(byte[] buffer, int index)
        {
            var statesFilterField = ImmutableArray<string>.Empty;
            var taggedFields = ImmutableArray<TaggedField>.Empty;
            return (index, new(
                statesFilterField,
                taggedFields
            ));
        }
        private static int WriteV3(byte[] buffer, int index, ListGroupsRequest message)
        {
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
        private static (int Offset, ListGroupsRequest Value) ReadV3(byte[] buffer, int index)
        {
            var statesFilterField = ImmutableArray<string>.Empty;
            var taggedFields = ImmutableArray<TaggedField>.Empty;
            (index, var taggedFieldsCount) = BinaryDecoder.ReadVarUInt32(buffer, index);
            if(taggedFieldsCount > 0)
            {
                var taggedFieldsBuilder = ImmutableArray.CreateBuilder<TaggedField>();
                while (taggedFieldsCount > 0)
                {
                    (index, var tag) = BinaryDecoder.ReadVarInt32(buffer, index);
                    (index, var bytes) = BinaryDecoder.ReadCompactBytes(buffer, index);
                    taggedFieldsBuilder.Add(new(tag, bytes));
                    taggedFieldsCount--;
                }
            }
            return (index, new(
                statesFilterField,
                taggedFields
            ));
        }
        private static int WriteV4(byte[] buffer, int index, ListGroupsRequest message)
        {
            index = BinaryEncoder.WriteCompactArray<string>(buffer, index, message.StatesFilterField, BinaryEncoder.WriteCompactString);
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
        private static (int Offset, ListGroupsRequest Value) ReadV4(byte[] buffer, int index)
        {
            var statesFilterField = ImmutableArray<string>.Empty;
            var taggedFields = ImmutableArray<TaggedField>.Empty;
            (index, var _statesFilterField_) = BinaryDecoder.ReadCompactArray<string>(buffer, index, BinaryDecoder.ReadCompactString);
            if (_statesFilterField_ == null)
                throw new NullReferenceException("Null not allowed for 'StatesFilter'");
            else
                statesFilterField = _statesFilterField_.Value;
            (index, var taggedFieldsCount) = BinaryDecoder.ReadVarUInt32(buffer, index);
            if(taggedFieldsCount > 0)
            {
                var taggedFieldsBuilder = ImmutableArray.CreateBuilder<TaggedField>();
                while (taggedFieldsCount > 0)
                {
                    (index, var tag) = BinaryDecoder.ReadVarInt32(buffer, index);
                    (index, var bytes) = BinaryDecoder.ReadCompactBytes(buffer, index);
                    taggedFieldsBuilder.Add(new(tag, bytes));
                    taggedFieldsCount--;
                }
            }
            return (index, new(
                statesFilterField,
                taggedFields
            ));
        }
    }
}