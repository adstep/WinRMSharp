using System.Net;

namespace WinRMSharp
{
    /// <summary>
    /// Options used to configure a <see cref="WinRMClient"/> instance.
    /// </summary>
    public class WinRMClientOptions
    {
        /// <summary>
        /// Maximum allowed time in seconds for any single wsman HTTP operation
        /// </summary>
        public TimeSpan? OperationTimeout { get; set; }

        /// <summary>
        /// Maximum response size in bytes. 
        /// </summary>
        public int? MaxEnvelopeSize { get; set; }

        /// <summary>
        /// Maximum timeout to wait before an HTTP connect/read times out.
        /// </summary>
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
                MaxEnvelopeSize = options?.MaxEnvelopeSize
            };

            Transport transport = new Transport(baseUrl, credentials, transportOptions);
            Protocol = new Protocol(transport, protocolOptions);
        }

        public WinRMClient(IProtocol protocol)
        {
            Protocol = protocol;
        }

        /// <summary>
        /// Executes a command on the destination host.
        /// </summary>
        /// <param name="command">Command to execute on the destination host.</param>
        /// <param name="args">Array of arguments for the command</param>
        /// <returns></returns>
        public async Task<CommandState> RunCommand(string command, string[]? args = null)
        {
            string shellId = await Protocol.OpenShell();
            string commandId = await Protocol.RunCommand(shellId, command, args);

            CommandState state = await Protocol.PollCommandState(shellId, commandId);

            await Protocol.CloseCommand(shellId, commandId);
            await Protocol.CloseShell(shellId);

            return state;
        }
    }
}
