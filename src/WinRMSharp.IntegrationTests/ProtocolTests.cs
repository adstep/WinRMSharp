using System.Net;
using Xunit;

namespace WinRMSharp.IntegrationTests
{
    public class ProtocolTests
    {
        private readonly Protocol _protocol;

        public ProtocolTests()
        {
            string baseUrl = Environment.GetEnvironmentVariable("WINRM_BASE_URL") ?? throw new ArgumentNullException("WINRM_BASE_URL");
            string username = Environment.GetEnvironmentVariable("WINRM_USERNAME") ?? throw new ArgumentNullException("WINRM_USERNAME");
            string password = Environment.GetEnvironmentVariable("WINRM_PASSWORD") ?? throw new ArgumentNullException("WINRM_PASSWORD");

            ICredentials credentials = new NetworkCredential(username, password);
            ITransport transport = new Transport(baseUrl, credentials);

            ProtocolOptions protocolOptions = new ProtocolOptions()
            {
                OperationTimeout = TimeSpan.FromSeconds(5)
            };

            _protocol = new Protocol(transport, protocolOptions);
        }


        [Fact]
        public async Task OpenCloseShell()
        {
            string shellId = await _protocol.OpenShell();
            await _protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", shellId);
        }

        [Fact]
        public async Task RunCommandWithArgsAndCleanup()
        {
            string shellId = await _protocol.OpenShell();
            string commandId = await _protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });

            await _protocol.CloseCommand(shellId, commandId);
            await _protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", commandId);
        }

        [Fact]
        public async Task RunCommandWithoutArgsAndCleanup()
        {
            string shellId = await _protocol.OpenShell();
            string commandId = await _protocol.RunCommand(shellId, "hostname");

            await _protocol.CloseCommand(shellId, commandId);
            await _protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", commandId);
        }

        [Fact]
        public async Task RunCommandWithEnv()
        {
            Dictionary<string, string> envVars = new Dictionary<string, string>()
            {
                { "TESTENV1", "hi mom" },
                { "TESTENV2", "another var" }
            };

            string shellId = await _protocol.OpenShell(envVars: envVars);
            string commandId = await _protocol.RunCommand(shellId, "echo", new string[] { "%TESTENV1%", "%TESTENV2%" });

            CommandState state = await _protocol.PollCommandState(shellId, commandId);

            await _protocol.CloseCommand(shellId, commandId);
            await _protocol.CloseShell(shellId);

            Assert.Matches(@"hi mom another var", state.Stdout);
        }

        [Fact]
        public async Task GetCommandState()
        {
            string shellId = await _protocol.OpenShell();
            string commandId = await _protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });

            CommandState state = await _protocol.PollCommandState(shellId, commandId);

            await _protocol.CloseCommand(shellId, commandId);
            await _protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Contains(@"Windows IP Configuration", state.Stdout);
            Assert.Equal(0, state.Stderr.Length);
        }

        [Fact]
        public async Task RunCommandExceedingOperationTimeout()
        {
            string shellId = await _protocol.OpenShell();
            string commandId = await _protocol.RunCommand(shellId, $"powershell -Command Start-Sleep -s {_protocol.OperationTimeout.TotalSeconds * 2}");

            CommandState state = await _protocol.PollCommandState(shellId, commandId);

            await _protocol.CloseCommand(shellId, commandId);
            await _protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Equal(0, state.Stderr.Length);
        }
    }
}
