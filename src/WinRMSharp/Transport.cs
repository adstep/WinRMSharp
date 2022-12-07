using System.Net;
using System.Text;

namespace WinRMSharp
{
    public class Transport : ITransport
    {
        private HttpClient _httpClient;

        public Transport(string baseUrl, ICredentials credentials)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                Credentials = credentials,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<string> Send(string message)
        {
            StringContent data = new StringContent(message, Encoding.UTF8, "application/soap+xml");

            HttpResponseMessage response = await _httpClient.PostAsync("wsman", data).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
