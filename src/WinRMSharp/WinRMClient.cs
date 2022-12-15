using System.Net;

namespace WinRMSharp
{
    public class WinRMClientOptions
    {
        public TimeSpan? OperationTimeout { get; set; }
        public int? MaxEnvelopeSize { get; set; }
        public string? Locale { get; set; }

        public TimeSpan? ReadTimeout { get; set; }
    }

    public class WinRMClient
    {
        public ITransport Transport => Protocol.Transport;
        public IProtocol Protocol { get; private set; }

        public WinRMClient(string baseUrl, ICredentials credentials, WinRMClientOptions? options = null)
        {
            TransportOptions transportOptions = new TransportOptions()
            {
                ReadTimeout = options?.OperationTimeout
            };

            ProtocolOptions protocolOptions = new ProtocolOptions
            {
                OperationTimeout = options?.OperationTimeout,
                MaxEnvelopeSize = options?.MaxEnvelopeSize,
                Locale = options?.Locale
            };

            Transport transport = new Transport(baseUrl, credentials, transportOptions);
            Protocol = new Protocol(transport, protocolOptions);
        }

        public WinRMClient(IProtocol protocol)
        {
            Protocol = protocol;
        }

        public async Task<CommandState> RunCommand(string command, string[]? args = null)
        {
            string shellId = await Protocol.OpenShell();
            string commandId = await Protocol.RunCommand(shellId, command, args);

            CommandState state = await Protocol.PollCommandState(shellId, commandId);

            await Protocol.CleanupCommand(shellId, commandId);
            await Protocol.CloseShell(shellId);

            return state;
        }
    }
}
