namespace WinRMSharp.Tests.Sessions
{
    public enum SessionState
    {
        Recording,
        Playback,
        PassThru
    }

    internal class SessionManager : IDisposable
    {
        private readonly List<SessionHandler> _handlers = new List<SessionHandler>();

        public void Dispose()
        {
            foreach (DelegatingHandler handler in _handlers)
            {
                if (handler is RecordingHandler recordingHandler)
                {
                    recordingHandler.Save();
                }
            }
        }

        public SessionHandler GenerateSessionHandler(
            SessionState state,
            string sessionName,
            Dictionary<string, string>? replacements = null)
        {
            SessionHandler? handler;

            if (state == SessionState.Recording)
            {
                handler = RecordingHandler.Create(sessionName, replacements);
            }
            else if (state == SessionState.Playback)
            {
                handler = PlaybackHandler.Load(sessionName);
            }
            else if (state == SessionState.PassThru)
            {
                handler = new PassThruHandler();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported session state '{state}'");
            }

            _handlers.Add(handler);
            return handler;
        }
    }
}
