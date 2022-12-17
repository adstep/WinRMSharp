using Xunit;

namespace WinRMSharp.IntegrationTests
{
    public abstract class BaseProtocolTests
    {
        [Fact]
        public async Task OpenCloseShell()
        {
            Protocol protocol = GenerateProtocol(nameof(OpenCloseShell));

            string shellId = await protocol.OpenShell();
            await protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", shellId);
        }

        [Fact]
        public async Task RunCommandWithArgsAndCleanup()
        {
            Protocol protocol = GenerateProtocol(nameof(RunCommandWithArgsAndCleanup));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", commandId);
        }

        [Fact]
        public async Task RunCommandWithoutArgsAndCleanup()
        {
            Protocol protocol = GenerateProtocol(nameof(RunCommandWithoutArgsAndCleanup));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "hostname");

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", commandId);
        }

        [Fact]
        public async Task RunCommandWithEnv()
        {
            Protocol protocol = GenerateProtocol(nameof(RunCommandWithEnv));

            Dictionary<string, string> envVars = new Dictionary<string, string>()
            {
                { "TESTENV1", "hi mom" },
                { "TESTENV2", "another var" }
            };

            string shellId = await protocol.OpenShell(envVars: envVars);
            string commandId = await protocol.RunCommand(shellId, "echo", new string[] { "%TESTENV1%", "%TESTENV2%" });

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Matches(@"hi mom another var", state.Stdout);
        }

        [Fact]
        public async Task GetCommandState()
        {
            Protocol protocol = GenerateProtocol(nameof(GetCommandState));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Contains(@"Windows IP Configuration", state.Stdout);
            Assert.Equal(0, state.Stderr.Length);
        }

        [Fact]
        public async Task RunCommandExceedingOperationTimeout()
        {
            Protocol protocol = GenerateProtocol(nameof(RunCommandExceedingOperationTimeout));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, $"powershell -Command Start-Sleep -s {protocol.OperationTimeout.TotalSeconds * 2}");

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Equal(0, state.Stderr.Length);
        }

        public abstract Protocol GenerateProtocol(string sessionName);
    }
}
