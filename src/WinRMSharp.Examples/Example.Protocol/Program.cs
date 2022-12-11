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

        bool enableMessageLogging = false;

        ICredentials credentials = new NetworkCredential()
        {
            UserName = userName,
            Password = password
        };

        TransportOptions transportOptions = new TransportOptions()
        {
            Handlers = !enableMessageLogging ? null : new DelegatingHandler[] { new MessageLoggingHandler() }
        };

        ITransport transport = new Transport(baseUrl, credentials, transportOptions);
        Protocol protocol = new Protocol(transport);

        string shellId = await protocol.OpenShell();
        Console.WriteLine($"Opened shell '{shellId}'");

        string commandId = await protocol.RunCommand(shellId, "ipconfig", new string[] { "/all" });
        Console.WriteLine($"Started command '{commandId}'");

        CommandState commandState = await protocol.GetCommandState(shellId, commandId);

        Console.WriteLine($"StatusCode: {commandState.StatusCode}");
        Console.WriteLine($"Stdout: \r\n{commandState.Stdout}");
        Console.WriteLine($"Stderr: \r\n{commandState.Stderr}");
    }
}

public class MessageLoggingHandler : DelegatingHandler
{
    private static int RequestCounter = 0;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int requestCount = ++RequestCounter;

        string requestContent = (request.Content == null) ? "NO CONTENT" : FormatXml(await request.Content!.ReadAsStringAsync());
        Console.WriteLine($"Request #{requestCount}:");
        Console.WriteLine(requestContent);

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        string responseContent = (response.Content == null) ? "NO CONTENT" : FormatXml(await response.Content.ReadAsStringAsync());
        Console.WriteLine($"Response #{requestCount}:");
        Console.WriteLine(responseContent);

        return response;
    }

    private string FormatXml(string xml)
    {
        try
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.ToString();
        }
        catch (Exception)
        {
            return xml;
        }
    }
}