using WinRMSharp.Contracts;
using WinRMSharp.Exceptions;
using Xunit;

namespace WinRMSharp.Tests
{
    public abstract class BaseProtocolTests
    {
        [Fact]
        public async Task ProtocolOpenCloseShell()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolOpenCloseShell));

            string shellId = await protocol.OpenShell();
            await protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", shellId);
        }

        [Fact]
        public async Task ProtocolGetCommandStateBeforeRun()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolGetCommandStateBeforeRun));

            string shellId = await protocol.OpenShell();
            string commandId = "de969dd4-2798-4a3c-9d57-28c1da9b2aa3";

            WSManFaultException wsManFault = await Assert.ThrowsAsync<WSManFaultException>(async () => await protocol.GetCommandState(shellId, commandId));

            Assert.Equal(Fault.INVALID_SELECTORS, wsManFault.Code);
            Assert.Equal("windows-host", wsManFault.Machine);
            Assert.Equal("The Windows Remote Shell received a request to perform an operation on a command identifier that does not exist. Either the command has completed execution or the client specified an invalid command identifier.", wsManFault.FaultMessage);
            Assert.Equal("The WS-Management service cannot process the request because the request contained invalid selectors for the resource.", wsManFault.Reason);
        }


        [Fact]
        public async Task ProtocolRunCommandWithArgsAndCleanup()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolRunCommandWithArgsAndCleanup));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", commandId);
        }

        [Fact]
        public async Task ProtocolRunCommandWithoutArgsAndCleanup()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolRunCommandWithoutArgsAndCleanup));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "hostname");

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Matches(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$", commandId);
        }

        [Fact]
        public async Task ProtocolRunCommandWithEnv()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolRunCommandWithEnv));

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
        public async Task ProtocolRunCommandWithUnicode()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolRunCommandWithUnicode));

            string shellId = await protocol.OpenShell(codePage: 65001);
            string commandId = await protocol.RunCommand(shellId, "powershell.exe", new string[] { "Write-Host こんにちは" });

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Equal("こんにちは\n", state.Stdout);
            Assert.Equal(string.Empty, state.Stderr);
        }

        [Fact]
        public async Task ProtocolRunCommandWithNoProfile()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolRunCommandWithNoProfile));

            string shellId = await protocol.OpenShell(noProfile: true);
            string commandId = await protocol.RunCommand(shellId, "cmd.exe", new string[] { "/c", "set" });

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Contains(state.Stdout.Split(Environment.NewLine), l => l.Contains("USERPROFILE=C:\\Users\\Default"));
            Assert.Equal(string.Empty, state.Stderr);
        }

        [Fact]
        public async Task ProtocolGetCommandState()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolGetCommandState));

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
        public async Task ProtocolRunCommandExceedingOperationTimeout()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolRunCommandExceedingOperationTimeout));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, $"powershell -Command Start-Sleep -s {protocol.OperationTimeout.TotalSeconds * 2}");

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Equal(0, state.Stderr.Length);
        }

        [Fact]
        public async Task ProtocolRunCommandWithCommandInput()
        {
            Protocol protocol = GenerateProtocol(nameof(ProtocolRunCommandWithCommandInput));

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "cmd");
            await protocol.SendCommandInput(shellId, commandId, "echo \"hello world\" && exit\r\n");

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Contains("hello world", state.Stdout);
            Assert.Empty(state.Stderr);
        }

        public abstract Protocol GenerateProtocol(string sessionName);
    }
}
