using System.Net;

namespace WinRMSharp
{
    public class WinRMClientOptions
    {
        public TimeSpan? OperationTimeout { get; set; }
        public int? MaxEnvelopeSize { get; set; }
        public string? Locale { get; set; }

        public TimeSpan? ReadTimeout { get; set; }
        public DelegatingHandler[]? Handlers { get; set; }
    }

    public class WinRMClient
    {
        private Transport _transport;
        private Protocol _protocol;

        public WinRMClient(string baseUrl, ICredentials credentials, WinRMClientOptions? options = null)
        {
            TransportOptions transportOptions = new TransportOptions()
            {
                ReadTimeout = options?.OperationTimeout,
                Handlers = options?.Handlers
            };

            ProtocolOptions protocolOptions = new ProtocolOptions
            {
                OperationTimeout = options?.OperationTimeout,
                MaxEnvelopeSize = options?.MaxEnvelopeSize,
                Locale = options?.Locale
            };

            _transport = new Transport(baseUrl, credentials, transportOptions);
            _protocol = new Protocol(_transport, protocolOptions);
        }

        public async Task<CommandState> RunCommand(string command, string[]? args = null)
        {
            string shellId = await _protocol.OpenShell();
            string commandId = await _protocol.RunCommand(shellId, command, args);

            CommandState state = await _protocol.PollCommandState(shellId, commandId);

            await _protocol.CleanupCommand(shellId, commandId);
            await _protocol.CloseShell(shellId);

            return state;
        }
    }
}
