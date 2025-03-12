using System.Net;
using WinRMSharp.Tests.Sessions;
using Xunit;

namespace WinRMSharp.Tests
{
    [Trait("Category", "Integration")]
    public class IntegrationClientTests : BaseClientTests, IDisposable
    {
        private readonly SessionManager _sessionManager = new SessionManager();

        /// <inheritdoc/>
        public void Dispose()
        {
            _sessionManager.Dispose();
        }

        /// <inheritdoc/>
        public override WinRMClient GenerateClient(string sessionName)
        {
            Uri baseUrl = new Uri(Environment.GetEnvironmentVariable("WINRM_BASE_URL") ?? throw new ArgumentNullException("WINRM_BASE_URL"));
            string username = Environment.GetEnvironmentVariable("WINRM_USERNAME") ?? throw new ArgumentNullException("WINRM_USERNAME");
            string password = Environment.GetEnvironmentVariable("WINRM_PASSWORD") ?? throw new ArgumentNullException("WINRM_PASSWORD");

            Tuple<string, string, bool>[] replacements = new Tuple<string, string, bool>[]
            {
                Tuple.Create(baseUrl.ToString(), "https://127.0.0.1:5986/", false),
                Tuple.Create(@"<rsp:ClientIP>\d{1,3}(?:\.\d{1,3}){3}</rsp:ClientIP>", "<rsp:ClientIP>127.0.0.1</rsp:ClientIP>", true),
                Tuple.Create(username, "exampleUser", false),
            };

            SessionHandler handler = _sessionManager.GenerateSessionHandler(
                SessionState.Recording,
                sessionName,
                replacements);

            ICredentials credentials = new NetworkCredential(username, password);
            ITransport transport = new Transport(baseUrl, handler, credentials);

            ProtocolOptions protocolOptions = new ProtocolOptions()
            {
                OperationTimeout = TimeSpan.FromSeconds(5)
            };

            Protocol protocol = new Protocol(transport, new IncrementingGuidProvider(), protocolOptions);

            return new WinRMClient(protocol);
        }
    }
}
