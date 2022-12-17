using WinRMSharp.Utils;

namespace WinRMSharp.IntegrationTests.Recording
{
    internal class RecordedResponse
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string? Body { get; set; }

        public RecordedResponse() { }

        public RecordedResponse(HttpResponseMessage responseMessage)
        {
            StatusCode = (int)responseMessage.StatusCode;
            Headers = responseMessage.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
            Body = Xml.Format(responseMessage.Content?.ReadAsStringAsync().Result);
        }
    }
}
