using System.Diagnostics;

namespace WinRMSharp.IntegrationTests.Recording
{
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

        public SessionHandler GenerateSessionHandler(State state, string sessionName)
        {
            SessionHandler? handler;

            if (state == State.Recording)
            {
                handler = RecordingHandler.Create(sessionName);
            }
            else if (state == State.Playback)
            {
                handler = PlaybackHandler.Load(sessionName);
            }
            else if (state == State.PassThru)
            {
                handler = new PassThruHandler();
            }
            else
            {
                throw new UnreachableException();
            }

            _handlers.Add(handler);
            return handler;
        }
    }
}
