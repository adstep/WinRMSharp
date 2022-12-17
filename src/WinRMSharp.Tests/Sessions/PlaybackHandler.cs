using System.Net;
using System.Xml.Linq;
using WinRMSharp.Utils;
using YamlDotNet.Serialization;

namespace WinRMSharp.Tests.Sessions
{
    internal class PlaybackHandler : SessionHandler
    {
        private int _playbackIndex = 0;
        private readonly List<Recording> _recordings = new List<Recording>();

        private PlaybackHandler(List<Recording> recordings)
        {
            _recordings = recordings;
        }

        public static PlaybackHandler Load(string sessionName)
        {
            string path = Path.Join(Directory.GetCurrentDirectory(), "Data", "Sessions", $"{sessionName}.yml");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Expected recording to exist at '{path}'");
            }

            using StreamReader fileStream = File.OpenText(path);

            IDeserializer deserializer = new DeserializerBuilder().Build();
            List<Recording> recordings = deserializer.Deserialize<List<Recording>>(fileStream);

            return new PlaybackHandler(recordings);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Recording recording = _recordings[_playbackIndex++];

            XDocument? requestDocument = (request.Content == null) ? null : Xml.Parse(await request.Content!.ReadAsStringAsync());
            XDocument? recordingDocument = (recording.Request?.Body == null) ? null : Xml.Parse(recording.Request!.Body);

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

    }
}
