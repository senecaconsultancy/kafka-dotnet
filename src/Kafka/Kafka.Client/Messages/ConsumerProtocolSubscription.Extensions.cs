using Kafka.Common.Encoding;
using System.CodeDom.Compiler;
namespace Kafka.Client.Messages.Extensions
{
    [GeneratedCode("kgen", "1.0.0.0")]
    public static class ConsumerProtocolSubscriptionExtensions
    {
        public static void Write(this ConsumerProtocolSubscription message, MemoryStream buffer)
        {
            Encoder.WriteArray(buffer, message.TopicsField, (b, i) =>
            {
                Encoder.WriteString(buffer, i);
                return 0;
            });
            Encoder.WriteBytes(buffer, message.UserDataField);
            Encoder.WriteArray(buffer, message.OwnedPartitionsField, (b, i) =>
            {
                Encoder.WriteString(buffer, i.TopicField);
                Encoder.WriteArray(buffer, i.PartitionsField, (b, i) =>
                {
                    Encoder.WriteInt32(buffer, i);
                    return 0;
                });
                return 0;
            });
        }
    }
}
