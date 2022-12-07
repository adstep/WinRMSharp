using System.Net;
using Xunit;

namespace WinRMSharp.IntegrationTests
{
    public class ProtocolTests
    {
        private Protocol _protocol;

        public ProtocolTests()
        {
            string? endpoint = Environment.GetEnvironmentVariable("WINRM_ENDPOINT");
            string? baseUrl = Environment.GetEnvironmentVariable("WINRM_BASE_URL");
            string? username = Environment.GetEnvironmentVariable("WINRM_USERNAME");
            string? password = Environment.GetEnvironmentVariable("WINRM_PASSWORD");

            ICredentials credentials = new NetworkCredential(username, password);
            ITransport transport = new Transport(baseUrl, credentials);

            _protocol = new Protocol(transport);
        }


        [Fact]
        public async Task OpenCloseShell()
        {
            string shellId = await _protocol.OpenShell();
            await _protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", shellId);
        }
    }
}