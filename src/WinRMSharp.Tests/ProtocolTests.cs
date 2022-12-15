using Moq;
using Xunit;

namespace WinRMSharp.Tests
{
    public class ProtocolTests
    {
        [Fact]
        public async Task OpenCloseShell()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);

            string shellId = await protocol.OpenShell();
            await protocol.CloseShell(shellId);

            Assert.Equal("11111111-1111-1111-1111-111111111113", shellId);
        }

        [Fact]
        public async Task RunCommandWithArgsAndCleanup()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new[] { "/all" });

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal("11111111-1111-1111-1111-111111111114", commandId);
        }

        [Fact]
        public async Task RunCommandWithoutArgsAndCleanup()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "hostname");

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal("11111111-1111-1111-1111-111111111114", commandId);
        }

        [Fact]
        public async Task GetCommandState()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new[] { "/all" });
            CommandState commandState = await protocol.GetCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.NotNull(commandState);
            Assert.Equal(0, commandState.StatusCode);
            Assert.Contains("Windows IP Configuration", commandState.Stdout);
            Assert.Equal(0, commandState.Stderr.Length);
        }

        [Fact]
        public async Task PollCommandState()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "ipconfig", new[] { "/all" });
            CommandState commandState = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.NotNull(commandState);
            Assert.Equal(0, commandState.StatusCode);
            Assert.Contains("Windows IP Configuration", commandState.Stdout);
            Assert.Equal(0, commandState.Stderr.Length);
        }

        [Fact]
        public async Task SendCommandInput()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, "cmd");
            await protocol.SendCommandInput(shellId, commandId, "echo \"hello world\" && exit\\r\\n");
            CommandState commandState = await protocol.GetCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.NotNull(commandState);
            Assert.Equal(0, commandState.StatusCode);
            Assert.Contains("hello world", commandState.Stdout);
            Assert.Equal(0, commandState.Stderr.Length);
        }

        [Fact]
        public async Task RunCommandExceedingOperationTimeout()
        {
            MockClient mockClient = new MockClient();
            Mock<IGuidProvider> guidProvider = new Mock<IGuidProvider>(MockBehavior.Strict);
            guidProvider.SetupSequence(s => s.NewGuid())
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"))  // OpenShell
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"))  // RunCommand
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"))  // First Poll
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111112"))  // Second Poll
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"))  // CleanupCommand
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111")); // CloseShell

            Protocol protocol = new Protocol(mockClient.Transport.Object, guidProvider.Object);

            string shellId = await protocol.OpenShell();
            string commandId = await protocol.RunCommand(shellId, $"powershell -Command Start-Sleep -s {protocol.OperationTimeout.TotalSeconds * 2}");

            CommandState state = await protocol.PollCommandState(shellId, commandId);

            await protocol.CloseCommand(shellId, commandId);
            await protocol.CloseShell(shellId);

            Assert.Equal(0, state.StatusCode);
            Assert.Equal(0, state.Stdout.Length);
            Assert.Equal(0, state.Stderr.Length);
        }

        [Fact]
        public async Task HandleCleanupCommandFault()
        {
            MockClient mockClient = new MockClient();
            Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);

            await protocol.CloseCommand("11111111-1111-1111-1111-111111111113", "11111111-1111-1111-1111-111111111117");

            // We've setup the transport to return a failure and we're just checcking an exception doesn't bubble up
        }
    }
}
