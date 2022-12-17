using YamlDotNet.Serialization;

namespace WinRMSharp.Tests.Sessions
{
    internal class RecordingHandler : SessionHandler
    {
        private readonly string _sessionName;
        private readonly List<Recording> _recordings = new List<Recording>();

        private RecordingHandler(string sessionName)
        {
            _sessionName = sessionName;
        }

        public static RecordingHandler Create(string sessionName)
        {
            return new RecordingHandler(sessionName);
        }

        public void Save()
        {
            ISerializer serializer = new SerializerBuilder().Build();
            string yaml = serializer.Serialize(_recordings);
            string path = Path.Join("Data", "Sessions", $"{_sessionName}.yml");

            if (!Directory.Exists("sessions"))
                Directory.CreateDirectory("sessions");

            File.WriteAllText(path, yaml);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            AddRecording(new Recording()
            {
                Request = new RecordedRequest(request),
                Response = new RecordedResponse(response)
            });

            return response;
        }

        private void AddRecording(Recording recording)
        {
            _recordings.Add(recording);
        }
    }
}
