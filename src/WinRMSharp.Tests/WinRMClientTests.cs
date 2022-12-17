using System.Net;
using Xunit;

namespace WinRMSharp.Tests
{
    //public class WinRMClientTests
    //{
    //    [Fact]
    //    public void VerifyConstruction()
    //    {
    //        string baseUrl = "https://localhost:5986";
    //        ICredentials credentials = new NetworkCredential()
    //        {
    //            Domain = "localhost",
    //            UserName = "testUser",
    //            Password = "testPassword"
    //        };
    //        WinRMClientOptions clientOptions = new WinRMClientOptions()
    //        {
    //            MaxEnvelopeSize = 1,
    //            OperationTimeout = TimeSpan.FromSeconds(20),
    //            ReadTimeout = TimeSpan.FromSeconds(30)
    //        };

    //        WinRMClient winRMClient = new WinRMClient(baseUrl, credentials, clientOptions);

    //        Assert.NotNull(winRMClient);
    //    }

    //    [Fact]
    //    public async Task RunCommand()
    //    {
    //        MockClient mockClient = new MockClient();
    //        Protocol protocol = new Protocol(mockClient.Transport.Object, mockClient.GuidProvider.Object);
    //        WinRMClient winRMClient = new WinRMClient(protocol);

    //        CommandState commandState = await winRMClient.RunCommand("ipconfig", new[] { "/all" });

    //        Assert.NotNull(commandState);
    //        Assert.Equal(0, commandState.StatusCode);
    //        Assert.Contains("Windows IP Configuration", commandState.Stdout);
    //        Assert.Equal(0, commandState.Stderr.Length);
    //    }
    //}
}
