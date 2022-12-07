using System.Net;
using System.Text.Json;

namespace WinRMSharp.Demo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string userName = "";
            string password = "";
            string baseUrl = "";

            ICredentials credentials = new NetworkCredential()
            {
                Domain = "localhost",
                UserName = userName,
                Password = password
            };

            ITransport transport = new Transport(baseUrl, credentials);
            Protocol protocol = new Protocol(transport);

            string shellId = await protocol.OpenShell();
            Console.WriteLine($"Opened shell '{shellId}'");

            string commandId = await protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });
            Console.WriteLine($"Started command '{commandId}'");

            await Task.Delay(TimeSpan.FromSeconds(10));

            CommandState commandState = await protocol.GetCommandState(shellId, commandId);

            Console.WriteLine($"{JsonSerializer.Serialize(commandState)}");
        }
    }
}