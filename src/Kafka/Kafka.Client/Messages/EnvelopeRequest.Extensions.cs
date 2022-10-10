using Kafka.Common.Encoding;
using System.CodeDom.Compiler;
namespace Kafka.Client.Messages.Extensions
{
    [GeneratedCode("kgen", "1.0.0.0")]
    public static class EnvelopeRequestExtensions
    {
        public static void Write(this EnvelopeRequest message, MemoryStream buffer)
        {
            Encoder.WriteBytes(buffer, message.RequestDataField);
            Encoder.WriteBytes(buffer, message.RequestPrincipalField);
            Encoder.WriteBytes(buffer, message.ClientHostAddressField);
        }
    }
}
