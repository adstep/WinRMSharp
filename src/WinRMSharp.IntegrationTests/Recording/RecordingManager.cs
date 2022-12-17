using System.Diagnostics;
using System.Net;
using System.Xml.Linq;
using WinRMSharp.Utils;
using YamlDotNet.Serialization;

namespace WinRMSharp.IntegrationTests.Recording
{
    public enum State
    {
        Recording,
        Playback,
        PassThru
    }

    internal class RecordingManager : DelegatingHandler
    {
        private int _playbackIndex = 0;
        private readonly List<Recording> _recordings = new List<Recording>();

        private readonly State _state = State.Playback;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_state == State.Recording)
            {
                return await Record(request, cancellationToken);
            }
            else if (_state == State.Playback)
            {
                return await Playback(request);
            }
            else if (_state == State.PassThru)
            {
                return await PassThru(request, cancellationToken);
            }
            else
            {
                throw new UnreachableException();
            }
        }

        private async Task<HttpResponseMessage> Record(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            AddRecording(new Recording()
            {
                Request = new RecordedRequest(request),
                Response = new RecordedResponse(response)
            });

            return response;
        }

        private async Task<HttpResponseMessage> Playback(HttpRequestMessage request)
        {
            Recording recording = _recordings[_playbackIndex++];

            XDocument? requestDocument = (request.Content == null) ? null : Xml.Parse(await request.Content!.ReadAsStringAsync());
            XDocument? recordingDocument = Xml.Parse(recording.Request.Body);

            if (!XNode.DeepEquals(requestDocument, recordingDocument))
            {
                throw new InvalidOperationException($"Expected '{requestDocument}' Actual: '{recordingDocument}'");
            }

            HttpResponseMessage response = new HttpResponseMessage((HttpStatusCode)recording.Response.StatusCode)
            {
                Content = new StringContent(recording.Response.Body!)
            };

            foreach (KeyValuePair<string, string> kv in recording.Response.Headers)
            {
                string key = kv.Key;
                string[] values = kv.Value.Split(';');

                response.Headers.Add(key, values);
            }

            return response;
        }

        private async Task<HttpResponseMessage> PassThru(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        public void Save()
        {
            string sessionName = "example";

            RecordedSession session = new RecordedSession()
            {
                Name = sessionName,
                Recordings = _recordings
            };

            ISerializer serializer = new SerializerBuilder().Build();
            string yaml = serializer.Serialize(session);
            string path = Path.Join("sessions", $"{sessionName}.yml");

            if (!Directory.Exists("sessions"))
                Directory.CreateDirectory("sessions");

            File.WriteAllText(path, yaml);
        }

        public void Load(string sessionName)
        {
            string path = Path.Join("sessions", $"{sessionName}.yml");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Expected recording to exist at '{path}'");
            }

            using StreamReader fileStream = File.OpenText(path);

            IDeserializer deserializer = new DeserializerBuilder().Build();
            RecordedSession session = deserializer.Deserialize<RecordedSession>(fileStream);

            _recordings.Clear();
            _recordings.AddRange(session.Recordings);
        }

        private void AddRecording(Recording recording)
        {
            _recordings.Add(recording);
        }
    }
}
