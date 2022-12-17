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
            string baseUrl = "https://127.0.0.1/wsman";
            string userName = "exampleUser";
            string password = "examplePassword";

            DelegatingHandler handler = _sessionManager.GenerateSessionHandler(SessionState.Playback, sessionName);

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
