using System.Net;
using WinRMSharp;

internal class Program
{
    public static async Task Main()
    {
        string baseUrl = "";
        string userName = "";
        string password = "";

        ICredentials credentials = new NetworkCredential()
        {
            UserName = userName,
            Password = password
        };

        WinRMClient client = new WinRMClient(baseUrl, credentials);

        CommandState commandState = await client.RunCommand("ipconfig", new string[] { "/all" });

        Console.WriteLine($"StatusCode: {commandState.StatusCode}");
        Console.WriteLine($"Stdout: \r\n{commandState.Stdout}");
        Console.WriteLine($"Stderr: \r\n{commandState.Stderr}");
    }
}
