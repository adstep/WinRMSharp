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
        /// <summary>
        /// Network transport used for sending/receiving SOAP requests/responses.
        /// </summary>
        public ITransport Transport => Protocol.Transport;

        /// <summary>
        /// WinRM protocol for managing sending/receiving SOAP requests/responses to facilitate.
        /// </summary>
        public IProtocol Protocol { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinRMClient"/> class.
        /// </summary>
        /// <param name="transport">Network transport used for sending/receiving SOAP requests/responses.</param>
        /// <param name="options">Options to configure instance.</param>
        public WinRMClient(string baseUrl, ICredentials credentials, WinRMClientOptions? options = null)
        {
            TransportOptions transportOptions = new TransportOptions()
            {
                ReadTimeout = options?.ReadTimeout
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
        /// <param name="args">Array of arguments for the command.</param>
        /// <param name="workingDirectory">Working directory of shell.</param>
        /// <param name="environment">Environment variables of shell.</param>
        /// <param name="idleTimeout">Time length before shell is closed.</param>
        /// <param name="codePage">Encoding of the output std buffers. Correlates to the codepage of the host. en-US traditionally maps to 437.</param>
        /// <param name="noProfile">Whether to create the shell with the user profile active or not.</param>
        /// <returns></returns>
        public async Task<CommandState> RunCommand(string command, string[]? args = null, string? workingDirectory = null, Dictionary<string, string>? environment = null, TimeSpan? idleTimeout = null, int? codePage = null, bool? noProfile = null)
        {
            string shellId = await Protocol.OpenShell(workingDirectory: workingDirectory, environment: environment, idleTimeout: idleTimeout, codePage: codePage, noProfile: noProfile);
            string commandId = await Protocol.RunCommand(shellId, command, args);

            CommandState state = await Protocol.PollCommandState(shellId, commandId);

            await Protocol.CloseCommand(shellId, commandId);
            await Protocol.CloseShell(shellId);

            return state;
        }
    }
}
