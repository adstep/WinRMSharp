using YamlDotNet.Serialization;

namespace WinRMSharp.Tests.Sessions
{
    internal class RecordingHandler : SessionHandler
    {
        private readonly string _sessionName;
        private readonly Tuple<string, string, bool>[]? _replacements;
        private readonly List<Recording> _recordings = new List<Recording>();

        private RecordingHandler(string sessionName, Tuple<string, string, bool>[]? replacements)
        {
            _sessionName = sessionName;
            _replacements = replacements;
        }

        public static RecordingHandler Create(string sessionName, Tuple<string, string, bool>[]? replacements)
        {
            return new RecordingHandler(sessionName, replacements);
        }

        public void Save()
        {
            ISerializer serializer = new SerializerBuilder()
                .WithTypeConverter(new SecretsMaskingConverter(_replacements))
                .Build();
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
