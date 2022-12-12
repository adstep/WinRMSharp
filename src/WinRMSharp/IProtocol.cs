namespace WinRMSharp
{
    public interface IProtocol
    {
        ITransport Transport { get; }

        Task<string> OpenShell(string inputStream = "stdin", string outputStream = "stdout stderr", string? workingDirectory = null, Dictionary<string, string>? envVars = null, TimeSpan? idleTimeout = null);
        Task<string> RunCommand(string shellId, string command, string[]? args = null);
        Task SendCommandInput(string shellId, string commandId, string input, bool end = false);
        Task<CommandState> PollCommandState(string shellId, string commandId);
        Task<CommandState> GetCommandState(string shellId, string commandId);
        Task CloseShell(string shellId);
        Task CleanupCommand(string shellId, string commandId);
    }
}
