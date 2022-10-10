using Kafka.Common.Encoding;
using System.CodeDom.Compiler;
namespace Kafka.Client.Messages.Extensions
{
    [GeneratedCode("kgen", "1.0.0.0")]
    public static class DeleteGroupsRequestExtensions
    {
        public static void Write(this DeleteGroupsRequest message, MemoryStream buffer)
        {
            Encoder.WriteArray(buffer, message.GroupsNamesField, (b, i) =>
            {
                Encoder.WriteString(buffer, i);
                return 0;
            });
        }
    }
}
