using WinRMSharp.Test;
using Xunit;

namespace WinRMSharp.Tests
{
    public class ProtocolTests
    {
        [Fact]
        public async Task OpenCloseShell()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = mockClient.Protocol.Object;

            string shellId = await protocol.OpenShell();
            await protocol.CloseShell(shellId);

            Assert.Equal("11111111-1111-1111-1111-111111111113", shellId);
        }

        [Fact]
        public async Task RunCommandWithArgsAndCleanup()
        {
            MockClient mockTransport = new MockClient();
            Protocol protocol = new Protocol(mockTransport.Transport.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new[] { "/all" });

            await protocol.CleanupCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal("11111111-1111-1111-1111-111111111114", commandId);
        }

        [Fact]
        public async Task RunCommandWithoutArgsAndCleanup()
        {
            MockClient mockTransport = new MockClient();
            Protocol protocol = new Protocol(mockTransport.Transport.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "hostname");

            await protocol.CleanupCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal("11111111-1111-1111-1111-111111111114", commandId);
        }

        [Fact]
        public async Task GetCommandState()
        {
            MockClient mockTransport = new MockClient();
            Protocol protocol = new Protocol(mockTransport.Transport.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new[] { "/all" });
            CommandState commandState = await protocol.GetCommandState(shellId, commandId);

            await protocol.CleanupCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.NotNull(commandState);
            Assert.Equal(0, commandState.StatusCode);
            Assert.Contains("Windows IP Configuration", commandState.Stdout);
            Assert.Equal(0, commandState.Stderr.Length);
        }

        [Fact]
        public async Task SendCommandInput()
        {
            MockClient mockTransport = new MockClient();
            Protocol protocol = new Protocol(mockTransport.Transport.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "cmd");
            await protocol.SendCommandInput(shellId, commandId, "echo \"hello world\" && exit\\r\\n");
            CommandState commandState = await protocol.GetCommandState(shellId, commandId);

            await protocol.CleanupCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.NotNull(commandState);
            Assert.Equal(0, commandState.StatusCode);
            Assert.Contains("hello world", commandState.Stdout);
            Assert.Equal(0, commandState.Stderr.Length);
        }
    }
}
