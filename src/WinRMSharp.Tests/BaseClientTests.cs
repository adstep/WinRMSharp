using Xunit;

namespace WinRMSharp.Tests
{
    public abstract class BaseClientTests
    {
        [Fact]
        public async Task ClientRunCommand()
        {
            WinRMClient client = GenerateClient(nameof(ClientRunCommand));

            CommandState state = await client.RunCommand("dir");

            Assert.NotNull(state);
            Assert.Equal(0, state.StatusCode);
            Assert.Contains("Volume in drive C", state.Stdout);
            Assert.Empty(state.Stderr);
        }

        [Fact]
        public async Task ClientRunCommandWithArgs()
        {
            WinRMClient client = GenerateClient(nameof(ClientRunCommandWithArgs));

            CommandState state = await client.RunCommand("echo", new string[] { "abc" });

            Assert.NotNull(state);
            Assert.Equal(0, state.StatusCode);
            Assert.Equal("abc\r\n", state.Stdout);
            Assert.Empty(state.Stderr);
        }

        [Fact]
        public async Task ClientRunCommandWithEnvironment()
        {
            WinRMClient client = GenerateClient(nameof(ClientRunCommandWithEnvironment));

            Dictionary<string, string> environment = new Dictionary<string, string>()
            {
                { "string", "string value" },
                { "int", "1234" },
                { "bool", "true" },
                { "double_quote", "double \" quote" },
                { "single_quote", "string ' value" },
                { "hyphen - var", "abc @ 123" },
            };

            CommandState state = await client.RunCommand("set", environment: environment);

            Assert.NotNull(state);
            IDictionary<string, string> actualEnvironment = state.Stdout.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(new[] { '=' }, 2))
                .ToDictionary(parts => parts[0], parts => parts[1]);

            foreach (KeyValuePair<string, string> kv in environment)
            {
                Assert.Contains(kv.Key, actualEnvironment);
                Assert.Equal(kv.Value, actualEnvironment[kv.Key]);
            }
        }

        public abstract WinRMClient GenerateClient(string sessionName);
    }
}
