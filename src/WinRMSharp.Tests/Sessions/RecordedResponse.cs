using WinRMSharp.Utils;

namespace WinRMSharp.Tests.Sessions
{
    internal class RecordedResponse
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string? Body { get; set; }

        public RecordedResponse()
        {
            StatusCode = 200;
            Headers = new Dictionary<string, string>();
            Body = null;
        }

        public RecordedResponse(HttpResponseMessage responseMessage)
        {
            StatusCode = (int)responseMessage.StatusCode;
            Headers = responseMessage.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value));

            if (responseMessage.Content != null)
            {
                Body = Xml.Format(responseMessage.Content!.ReadAsStringAsync().Result);
            }
        }
    }
}
