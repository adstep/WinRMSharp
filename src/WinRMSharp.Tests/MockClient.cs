using Moq;
using System.Xml.Linq;
using WinRMSharp.Exceptions;
using WinRMSharp.Utils;

namespace WinRMSharp.Tests
{
    public class MockClient
    {
        public Mock<IGuidProvider> GuidProvider { get; private set; }
        public Mock<ITransport> Transport { get; private set; }
        public Mock<Protocol> Protocol { get; private set; }

        public MockClient()
        {
            GuidProvider = new Mock<IGuidProvider>();
            GuidProvider.Setup(p => p.NewGuid()).Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));

            Transport = new Mock<ITransport>();
            Protocol = new Mock<Protocol>(Transport.Object, GuidProvider.Object, null);

            Transport.Setup(m => m.Send(It.IsAny<string>())).ReturnsAsync((string message) => ResolveResponse(message));
        }

        private string ResolveResponse(string message)
        {
            //message = Patch(message);

            XDocument env = message.Parse();

            // WARNING: Despite being contrary to the XML standard, DeepEquals only
            // evaluates two nodes as equal if all attributes are in matching order.
            // See https://github.com/dotnet/dotnet-api-docs/issues/830
            if (XNode.DeepEquals(env, Config.OPEN_SHELL_REQUEST.Parse()))
            {
                return Config.OPEN_SHELL_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.CLOSE_SHELL_REQUEST.Parse()))
            {
                return Config.CLOSE_SHELL_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.RUN_CMD_WITH_ARGS_REQUEST.Parse()) || XNode.DeepEquals(env, Config.RUN_CMD_WO_ARGS_REQUEST.Parse()))
            {
                return string.Format(Config.RUN_CMD_PS_RESPONSE, "1");
            }
            else if (XNode.DeepEquals(env, string.Format(Config.CLEANUP_CMD_REQUEST, "1").Parse()) ||
                XNode.DeepEquals(env, string.Format(Config.CLEANUP_CMD_REQUEST, "2").Parse()) ||
                XNode.DeepEquals(env, string.Format(Config.CLEANUP_CMD_REQUEST, "3").Parse()))
            {
                return Config.CLEANUP_CMD_RESPONSE;
            }
            else if (XNode.DeepEquals(env, string.Format(Config.GET_CMD_PS_OUTPUT_REQUEST, "1").Parse()))
            {
                return Config.GET_CMD_OUTPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, string.Format(Config.GET_CMD_PS_OUTPUT_REQUEST, "2").Parse()))
            {
                return Config.GET_PS_OUTPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.RUN_CMD_REQ_INPUT.Parse()))
            {
                return Config.RUN_CMD_REQ_INPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.RUN_CMD_SEND_INPUT.Parse()))
            {
                return Config.RUN_CMD_SEND_INPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.RUN_CMD_SEND_INPUT_GET_OUTPUT.Parse()))
            {
                return Config.RUN_CMD_SEND_INPUT_GET_OUTPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.STDIN_CMD_CLEANUP.Parse()))
            {
                return Config.STDIN_CMD_CLEANUP_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.OPERATION_TIMEOUT_REQUEST.Parse()))
            {
                return Config.OPERATION_TIMEOUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.OPERATION_TIMEOUT_GET_0_REQUEST.Parse()))
            {
                throw new TransportException(500, Config.OPERATION_TIMEOUT_GET_0_RESPONSE);
            }
            else if (XNode.DeepEquals(env, Config.OPERATION_TIMEOUT_GET_1_REQUEST.Parse()))
            {
                return Config.OPERATION_TIMEOUT_GET_1_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Config.CLOSE_COMMAND_FAULT_REQUEST.Parse()))
            {
                throw new TransportException(500, Config.CLOSE_COMMAND_FAULT_RESPONSE);
            }


            throw new Exception($"Unexpected request message: '{message}'");
        }

        //private static string Patch(string message)
        //{
        //    // Hardcode the guids to be consistent across all messages to simplify equality.
        //    return Regex.Replace(message, @"uuid:[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}", "uuid:11111111-1111-1111-1111-111111111111");
        //}
    }
}
