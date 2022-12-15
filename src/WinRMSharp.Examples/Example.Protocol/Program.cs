using System.Net;
using System.Xml.Linq;
using WinRMSharp;

internal class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "";
        string userName = "";
        string password = "";

        ICredentials credentials = new NetworkCredential()
        {
            UserName = userName,
            Password = password
        };

        ITransport transport = new Transport(baseUrl, credentials);
        Protocol protocol = new Protocol(transport);

        string shellId = await protocol.OpenShell();
        Console.WriteLine($"Opened shell '{shellId}'");

        string commandId = await protocol.RunCommand(shellId, $"ipconfig", new string[] {"/all"});
        Console.WriteLine($"Started command '{commandId}'");

        CommandState commandState = await protocol.PollCommandState(shellId, commandId);

        Console.WriteLine($"StatusCode: {commandState.StatusCode}");
        Console.WriteLine($"Stdout: \r\n{commandState.Stdout}");
        Console.WriteLine($"Stderr: \r\n{commandState.Stderr}");

        await protocol.CloseCommand(shellId, commandId);
        try
        {
            await protocol.CloseCommand("11111111-1111-1111-1111-111111111111", commandId);
        }
        catch { }
        await protocol.CloseShell(shellId);
    }
}