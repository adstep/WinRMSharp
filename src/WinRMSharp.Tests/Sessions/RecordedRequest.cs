namespace WinRMSharp.Tests.Sessions
{
    public class RecordedRequest
    {
        public string Method { get; set; }
        public string? Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string? Body { get; set; }

        public RecordedRequest()
        {
            Method = string.Empty;
            Url = string.Empty;
            Headers = new Dictionary<string, string>();
            Body = string.Empty;
        }

        public RecordedRequest(HttpRequestMessage requestMessage)
        {
            Method = requestMessage.Method.Method;
            Url = requestMessage.RequestUri?.ToString();
            Headers = ApplyFilters(requestMessage.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value)));
            Body = requestMessage.Content?.ReadAsStringAsync().Result;
        }

        private static Dictionary<string, string> ApplyFilters(Dictionary<string, string> headers)
        {
            HashSet<string> headersToFilter = new HashSet<string>()
            {
                "Authorization"
            };

            foreach (string headerToFilter in headersToFilter)
            {
                if (!headers.ContainsKey(headerToFilter))
                    continue;

                headers.Remove(headerToFilter);
            }

            return headers;
        }
    }
}
