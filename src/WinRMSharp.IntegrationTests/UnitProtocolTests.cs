using System.Net;
using WinRMSharp.IntegrationTests.Recording;

namespace WinRMSharp.IntegrationTests
{
    public class UnitProtocolTests : BaseProtocolTests, IDisposable
    {
        private SessionManager _recordingManager = new SessionManager();

        public void Dispose()
        {
            _recordingManager.Dispose();
        }

        public override Protocol GenerateProtocol(string sessionName)
        {
            string baseUrl = "https://127.0.0.1/wsman";
            string userName = "exampleUser";
            string password = "examplePassword";

            DelegatingHandler handler = _recordingManager.GenerateSessionHandler(State.Playback, sessionName);

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
