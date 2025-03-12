using System.Net;
using WinRMSharp.Tests.Sessions;
using Xunit;

namespace WinRMSharp.Tests
{
    [Trait("Category", "InMemory")]
    public class InMemoryProtocolTests : BaseProtocolTests, IDisposable
    {
        private readonly SessionManager _sessionManager = new SessionManager();

        /// <inheritdoc/>
        public void Dispose()
        {
            _sessionManager.Dispose();
        }

        /// <inheritdoc/>
        public override Protocol GenerateProtocol(string sessionName)
        {
            Uri baseUrl = new Uri("https://127.0.0.1:5986/wsman");
            string userName = "exampleUser";
            string password = "examplePassword";

            Tuple<string, string, bool>[] replacements = new Tuple<string, string, bool>[]
            {
                Tuple.Create(baseUrl.ToString(), "https://127.0.0.1:5986/wsman", false),
                Tuple.Create(baseUrl.Host, "127.0.0.1", false),
                Tuple.Create(userName, "exampleUser", false),
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
