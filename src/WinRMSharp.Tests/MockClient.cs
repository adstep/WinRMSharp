using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using WinRMSharp.Utils;

namespace WinRMSharp.Test
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
            message = Patch(message);

            XDocument env = Xml.Parse(message);

            // WARNING: Despite being contrary to the XML standard, DeepEquals only
            // evaluates two nodes as equal if all attributes are in matching order.
            // See https://github.com/dotnet/dotnet-api-docs/issues/830
            if (XNode.DeepEquals(env, Xml.Parse(Config.OPEN_SHELL_REQUEST)))
            {
                return Config.OPEN_SHELL_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(Config.CLOSE_SHELL_REQUEST)))
            {
                return Config.CLOSE_SHELL_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(Config.RUN_CMD_WITH_ARGS_REQUEST)) || XNode.DeepEquals(env, Xml.Parse(Config.RUN_CMD_WO_ARGS_REQUEST)))
            {
                return string.Format(Config.RUN_CMD_PS_RESPONSE, "1");
            }
            else if (XNode.DeepEquals(env, Xml.Parse(string.Format(Config.CLEANUP_CMD_REQUEST, "1"))) || XNode.DeepEquals(env, Xml.Parse(string.Format(Config.CLEANUP_CMD_REQUEST, "2"))))
            {
                return Config.CLEANUP_CMD_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(string.Format(Config.GET_CMD_PS_OUTPUT_REQUEST, "1"))))
            {
                return Config.GET_CMD_OUTPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(string.Format(Config.GET_CMD_PS_OUTPUT_REQUEST, "2"))))
            {
                return Config.GET_PS_OUTPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(Config.RUN_CMD_REQ_INPUT)))
            {
                return Config.RUN_CMD_REQ_INPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(Config.RUN_CMD_SEND_INPUT)))
            {
                return Config.RUN_CMD_SEND_INPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(Config.RUN_CMD_SEND_INPUT_GET_OUTPUT)))
            {
                return Config.RUN_CMD_SEND_INPUT_GET_OUTPUT_RESPONSE;
            }
            else if (XNode.DeepEquals(env, Xml.Parse(Config.STDIN_CMD_CLEANUP)))
            {
                return Config.STDIN_CMD_CLEANUP_RESPONSE;
            }

            throw new Exception($"Unexpected request message: '{message}'");
        }

        private static string Patch(string message)
        {
            // Hardcode the guids to be consistent across all messages to simplify equality.
            return Regex.Replace(message, @"uuid:[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}", "uuid:11111111-1111-1111-1111-111111111111");
        }
    }
}
