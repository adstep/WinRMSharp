using System.Net;
using WinRMSharp.IntegrationTests.Recording;

namespace WinRMSharp.IntegrationTests
{
    public class IntegrationProtocolTests : BaseProtocolTests, IDisposable
    {
        private SessionManager _sessionManager = new SessionManager();

        public IntegrationProtocolTests()
        {

        }

        public void Dispose()
        {
            _sessionManager.Dispose();
        }

        public override Protocol GenerateProtocol(string sessionName)
        {
            string baseUrl = Environment.GetEnvironmentVariable("WINRM_BASE_URL") ?? throw new ArgumentNullException("WINRM_BASE_URL");
            string username = Environment.GetEnvironmentVariable("WINRM_USERNAME") ?? throw new ArgumentNullException("WINRM_USERNAME");
            string password = Environment.GetEnvironmentVariable("WINRM_PASSWORD") ?? throw new ArgumentNullException("WINRM_PASSWORD");

            SessionHandler handler = _sessionManager.GenerateSessionHandler(State.Recording, sessionName);

            ICredentials credentials = new NetworkCredential(username, password);
            ITransport transport = new Transport(baseUrl, handler, credentials);

            ProtocolOptions protocolOptions = new ProtocolOptions()
            {
                OperationTimeout = TimeSpan.FromSeconds(5)
            };

            return new Protocol(transport, new IncrementingGuidProvider(), protocolOptions);
        }
    }
}
