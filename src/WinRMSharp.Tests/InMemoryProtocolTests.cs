using System.Net;
using WinRMSharp.Tests.Sessions;
using Xunit;

namespace WinRMSharp.Tests
{
    [Trait("Category", "InMemory")]
    public class InMemoryProtocolTests : BaseProtocolTests, IDisposable
    {
        private readonly SessionManager _sessionManager = new SessionManager();

        public void Dispose()
        {
            _sessionManager.Dispose();
        }

        public override Protocol GenerateProtocol(string sessionName)
        {
            Uri baseUrl = new Uri("https://127.0.0.1:5986/wsman");
            string userName = "exampleUser";
            string password = "examplePassword";

            Dictionary<string, string> replacements = new Dictionary<string, string>
            {
                { baseUrl.ToString(), "https://127.0.0.1:5986/wsman" },
                { baseUrl.Host, "127.0.0.1" },
                { userName, "exampleUser" },
            };

            DelegatingHandler handler = _sessionManager.GenerateSessionHandler(
                SessionState.Playback,
                sessionName,
                replacements);

            ICredentials credentials = new NetworkCredential(userName, password);
            ITransport transport = new Transport(baseUrl, handler, credentials);

            ProtocolOptions protocolOptions = new ProtocolOptions()
            {
                OperationTimeout = TimeSpan.FromSeconds(5)
            };

            return new Protocol(transport, new IncrementingGuidProvider(), protocolOptions);
        }
    }
}
