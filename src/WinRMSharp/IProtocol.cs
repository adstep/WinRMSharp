namespace WinRMSharp
{
    /// <summary>
    /// Encapsulates WinRM network protocol for sending/receiving SOAP requests/responses necessary
    /// for executing remote commands.
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Network transport used for sending/receiving SOAP requests/responses.
        /// </summary>
        ITransport Transport { get; }

        /// <summary>
        /// Open a shell instance on the destination host.
        /// </summary>
        /// <param name="inputStream">Input streams to open.</param>
        /// <param name="outputStream">Output streams to open.</param>
        /// <param name="workingDirectory">Working directory of shell.</param>
        /// <param name="envVars">Environment variables of shell.</param>
        /// <param name="idleTimeout">Time length before shell is closed, if unused.</param>
        /// <returns>Identifier for opened shell.</returns>
        Task<string> OpenShell(string inputStream = "stdin", string outputStream = "stdout stderr", string? workingDirectory = null, Dictionary<string, string>? envVars = null, TimeSpan? idleTimeout = null);

        /// <summary>
        /// Run a command in an opened shell on the destination host.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="command">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        /// <param name="args">Array of arguments for the command.</param>
        /// <returns>Identifier for the executing command.</returns>
        Task<string> RunCommand(string shellId, string command, string[]? args = null);

        /// <summary>
        /// Send input to a command executing in a shell.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="command">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        /// <param name="input">Input text to send.</param>
        /// <param name="end">Boolean flag to indicate to close the stdin stream.</param>
        Task SendCommandInput(string shellId, string commandId, string input, bool end = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="command">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        Task<CommandState> PollCommandState(string shellId, string commandId);

        /// <summary>
        /// Gets the latest state of a command executing in a shell.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="command">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        /// <returns>State of a executing command.</returns>
        Task<CommandState> GetCommandState(string shellId, string commandId);

        /// <summary>
        /// Cleans up an executed command on the destination host.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="command">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        Task CloseCommand(string shellId, string commandId);

        /// <summary>
        /// Close a shell on the destination host.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        Task CloseShell(string shellId);
    }
}
