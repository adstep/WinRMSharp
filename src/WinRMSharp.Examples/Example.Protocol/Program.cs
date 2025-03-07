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

        ITransport transport = new Transport(baseUrl, credentials);
        Protocol protocol = new Protocol(transport);
        CommandState commandState;

        string shellId = await protocol.OpenShell();
        try
        {
            Console.WriteLine($"Opened shell '{shellId}'");

            string commandId = await protocol.RunCommand(shellId, $"ipconfig", new string[] { "/all" });

            try
            {
                Console.WriteLine($"Started command '{commandId}'");

                commandState = await protocol.PollCommandState(shellId, commandId);
            }
            finally
            {
                await protocol.CloseCommand(shellId, commandId);
            }
        }
        finally
        {
            await protocol.CloseShell(shellId);
        }

        Console.WriteLine($"StatusCode: {commandState.StatusCode}");
        Console.WriteLine($"Stdout: \r\n{commandState.Stdout}");
        Console.WriteLine($"Stderr: \r\n{commandState.Stderr}");
    }
}
