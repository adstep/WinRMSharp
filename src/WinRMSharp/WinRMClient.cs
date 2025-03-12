using System.Net;
using WinRMSharp.Exceptions;
using WinRMSharp.Utils;

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
        /// <param name="baseUrl">BaseUrl remote machine.</param>
        /// <param name="credentials">Credentials to use for communication with the remote machine.</param>
        /// <param name="options">Options to configure instance.</param>
        public WinRMClient(string baseUrl, ICredentials credentials, WinRMClientOptions? options = null)
            : this(new Uri(baseUrl), credentials, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinRMClient"/> class.
        /// </summary>
        /// <param name="baseUrl">BaseUrl remote machine.</param>
        /// <param name="credentials">Credentials to use for communication with the remote machine.</param>
        /// <param name="options">Options to configure instance.</param>
        public WinRMClient(Uri baseUrl, ICredentials credentials, WinRMClientOptions? options = null)
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

        internal WinRMClient(IProtocol protocol)
        {
            Protocol = protocol;
        }

        /// <summary>
        /// Copies a single file from the current machine to the remote machine.
        /// </summary>
        /// <param name="source">Path to a file on the local machine.</param>
        /// <param name="destination">Path to the destination file on the remote machine</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Thrown if source file can't be found.</exception>
        /// <exception cref="WinRMException">Thrown if file can't be copied.</exception>
        public async Task PutFile(string source, string destination)
        {
            if (!File.Exists(source))
                throw new FileNotFoundException($"Failed to put file on remote machine. Source file does not exist '{source}'.");

            string scriptTemplate =
$$$"""
begin {{
    $path = '{0}'

    $DebugPreference = "Continue"
    $ErrorActionPreference = "Stop"
    Set-StrictMode -Version 2

    $fd = [System.IO.File]::Create($path)

    $sha1 = [System.Security.Cryptography.SHA1CryptoServiceProvider]::Create()

    $bytes = @() #initialize for empty file case
}}
process {{
   $bytes = [System.Convert]::FromBase64String($input)
   $sha1.TransformBlock($bytes, 0, $bytes.Length, $bytes, 0) | Out-Null
   $fd.Write($bytes, 0, $bytes.Length)
}}
end {{
    $sha1.TransformFinalBlock($bytes, 0, 0) | Out-Null

    $hash = [System.BitConverter]::ToString($sha1.Hash).Replace("-", "").ToLowerInvariant()

    $fd.Close()

    Write-Output $hash
}}
""";
            string script = string.Format(scriptTemplate, destination);
            string command = Powershell.Command(script);
            CommandState state;

            string shellId = await Protocol.OpenShell();

            try
            {
                string commandId = await Protocol.RunCommand(shellId, command);

                try
                {
                    IEnumerable<(string, bool)> iterator = FileIterator(source);

                    foreach ((string data, bool isLast) in iterator)
                    {
                        await Protocol.SendCommandInput(shellId, commandId, data, isLast);
                    }

                    state = await Protocol.PollCommandState(shellId, commandId);
                }
                finally
                {
                    await Protocol.CloseCommand(shellId, commandId);
                }
            }
            finally
            {
                await Protocol.CloseShell(shellId);
            }

            if (state.StatusCode != 0)
            {
                string stderr = Powershell.ParseError(state.Stderr);
                throw new WinRMException($"Failed to copy file to remote machine. Stderr: {stderr}");
            }

            string remoteHash = state.Stdout.Trim();
            string localHash = Crypto.ComputeSecurehash(source);

            if (localHash != remoteHash)
                throw new WinRMException($"Failed to copy file to remote machine. Remote hash {remoteHash} does not match local hash {localHash}.");
        }

        /// <summary>
        /// Fetches a single file from the remote machine and creates a local copy.
        /// </summary>
        /// <param name="source">The path to the file on the remote machine to fetch.</param>
        /// <param name="destination">The path on the local machine to store the file.</param>
        /// <exception cref="WinRMException">Throws WinRMException on failure to fetch file.</exception>
        public async Task FetchFile(string source, string destination)
        {
            const int bufferSize = 524288; // 0.5 MB chunks

            if (Directory.Exists(destination))
            {
                return;
            }

            string scriptTemplate =
$$$"""
$path = '{0}'
If (Test-Path -Path $path -PathType Leaf)
{{
    $buffer_size = {1}
    $offset = {2}

    $stream = New-Object -TypeName IO.FileStream($path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
    $stream.Seek($offset, [System.IO.SeekOrigin]::Begin) > $null
    $buffer = New-Object -TypeName byte[] $buffer_size
    $bytes_read = $stream.Read($buffer, 0, $buffer_size)
    if ($bytes_read -gt 0) {{
        $bytes = $buffer[0..($bytes_read - 1)]
        [System.Convert]::ToBase64String($bytes)
    }}
    $stream.Close() > $null
}}
ElseIf (Test-Path -Path $path -PathType Container)
{{
    throw "The path at '$path' is a directory, source must be a file"
}}
Else
{{
    throw "The path at '$path' does not exist"
}}
""";

            int offset = 0;
            using FileStream fileStream = File.OpenWrite(destination);
            using BinaryWriter binaryWriter = new BinaryWriter(fileStream);

            while (true)
            {
                string script = string.Format(scriptTemplate, source, bufferSize, offset);
                string command = Powershell.Command(script);
                CommandState state;

                string shellId = await Protocol.OpenShell();
                try
                {
                    string commandId = await Protocol.RunCommand(shellId, command);

                    try
                    {
                        state = await Protocol.PollCommandState(shellId, commandId);
                    }
                    finally
                    {
                        await Protocol.CloseCommand(shellId, commandId);
                    }
                }
                finally
                {
                    await Protocol.CloseShell(shellId);
                }

                if (state.StatusCode != 0)
                {
                    string stderr = Powershell.ParseError(state.Stderr);
                    throw new WinRMException($"Failed to fetch file '{source}'. Stderr: {stderr}");
                }

                byte[]? buffer;

                if (state.Stdout.Trim() == "[DIR]")
                {
                    buffer = null;
                }
                else
                {
                    buffer = Convert.FromBase64String(state.Stdout);
                }

                if (buffer == null || buffer.Length == 0)
                {
                    break;
                }
                else
                {
                    binaryWriter.Write(buffer);
                }

                offset += buffer.Length;
            }
        }

        private IEnumerable<(string, bool)> FileIterator(string source, int bufferSize = 250000)
        {
            using FileStream fileStream = File.OpenRead(source);
            using BinaryReader binaryReader = new BinaryReader(fileStream);

            byte[] buffer = new byte[bufferSize];
            int bytesRead = binaryReader.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                // the file is empty
                yield return (string.Empty, true);
                yield break;
            }

            do
            {
                string base64Data = Convert.ToBase64String(buffer, 0, bytesRead) + "\r\n";
                yield return (base64Data, fileStream.Position == fileStream.Length);
            }
            while ((bytesRead = binaryReader.Read(buffer, 0, buffer.Length)) > 0);
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
        /// <param name="commandTimeout">The maximum allowed time for the command.</param>
        /// <returns></returns>
        public async Task<CommandState> RunCommand(
            string command,
            string[]? args = null,
            string? workingDirectory = null,
            Dictionary<string, string>? environment = null,
            TimeSpan? idleTimeout = null,
            int? codePage = null,
            bool? noProfile = null,
            TimeSpan? commandTimeout = null)
        {
            string shellId = await Protocol.OpenShell(workingDirectory: workingDirectory, environment: environment, idleTimeout: idleTimeout, codePage: codePage, noProfile: noProfile);
            CommandState state;

            try
            {
                string commandId = await Protocol.RunCommand(shellId, command, args, commandTimeout);

                try
                {
                    state = await Protocol.PollCommandState(shellId, commandId, commandTimeout);
                }
                finally
                {
                    await Protocol.CloseCommand(shellId, commandId);
                }
            }
            finally
            {
                await Protocol.CloseShell(shellId);
            }

            return state;
        }
    }
}
