using WinRMSharp.Exceptions;
using WinRMSharp.Tests.Utils;
using WinRMSharp.Utils;
using Xunit;

namespace WinRMSharp.Tests
{
    public abstract class BaseClientTests
    {
        [Fact]
        public async Task ClientPutFile_ZeroByteFile()
        {
            // Arrange
            using TemporaryFile tempFile = new TemporaryFile("test_file.txt");

            // create a zero byte file
            using (FileStream fileStream = File.Create(tempFile))
            {
                fileStream.Close();
            }

            WinRMClient client = GenerateClient(nameof(ClientPutFile_ZeroByteFile));
            string source = tempFile;
            string destination = "test_file.txt";

            // Act
            await client.PutFile(source, destination);

            // Assert
            CommandState state = await client.RunCommand($"powershell.exe Get-Content {destination}");

            Assert.Equal(string.Empty, state.Stdout.Trim());
        }

        [Fact]
        public async Task ClientPutFile()
        {
            using TemporaryFile tempFile = new TemporaryFile("test_file.txt");

            string testContent = "abcdefghijklmnopqrstuvwxyz";
            File.WriteAllText(tempFile, testContent);

            WinRMClient client = GenerateClient(nameof(ClientPutFile));
            string source = tempFile;
            string destination = "test_file.txt";

            await client.PutFile(source, destination);

            CommandState state = await client.RunCommand($"powershell.exe Get-Content {destination}");

            Assert.Equal(testContent, state.Stdout.Trim());
        }

        [Fact]
        public async Task ClientPutFileReallyLarge()
        {
            using TemporaryFile tempFile = new TemporaryFile("test_file_really_large.txt");

            string testContent = string.Join(Environment.NewLine, Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 20000));
            File.WriteAllText(tempFile, testContent);

            WinRMClient client = GenerateClient(nameof(ClientPutFileReallyLarge));
            string source = tempFile;
            string destination = "test_file_really_large.txt";

            await client.PutFile(source, destination);

            try
            {
                CommandState state = await client.RunCommand($"powershell.exe Get-Content {destination}");

                Assert.Equal(testContent, state.Stdout.Trim());
            }
            finally
            {
                await client.RunCommand($"powershell Remove-Item -Path '{destination}'");
            }
        }

        [Fact]
        public async Task ClientFetchFile()
        {
            using TemporaryFile tempFile = new TemporaryFile("test_fetch_file.txt");

            WinRMClient client = GenerateClient(nameof(ClientFetchFile));
            string source = @"C:\temp\file.txt";
            string destination = tempFile;

            string content = string.Join(Environment.NewLine, new string[]
            {
                @"New-Item -Path C:\temp\file.txt -Type file -Force",
                @"Set-Content -Path C:\temp\file.txt -Value (""abc`r`n"" * 50000)"
            });

            string command = Powershell.Command(content);
            CommandState state = await client.RunCommand(command);

            await client.FetchFile(source, destination);

            string expectedHash = "70e3bea8cdb0d0c883bccff5228933d933b88a80";
            string actualHash = Crypto.ComputeSecurehash(destination);

            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public async Task ClientFetchFileFailDir()
        {
            using TemporaryFile tempFile = new TemporaryFile("test_fetch_file.txt");

            WinRMClient client = GenerateClient(nameof(ClientFetchFileFailDir));
            string source = @"C:\Windows";
            string destination = tempFile;

            WinRMException ex = await Assert.ThrowsAsync<WinRMException>(async () => await client.FetchFile(source, destination));

            string errorMessage = "The path at 'C:\\Windows' is a directory, source must be a file";

            Assert.NotNull(ex);
            Assert.Contains(errorMessage, ex.Message);
        }

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
