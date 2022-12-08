using System.Net;
using System.Security.Authentication;
using System.Text;
using WinRMSharp.Exceptions;

namespace WinRMSharp
{
    public class TransportOptions
    {
        public TimeSpan? ReadTimeout { get; set; }
        public DelegatingHandler[]? Handlers { get; set; }
    }

    public class Transport : ITransport
    {
        private static TimeSpan DefaultReadTimeout = TimeSpan.FromSeconds(30);

        private List<HttpMessageHandler> _handlers;
        private HttpClient _httpClient;

        public Transport(string baseUrl, ICredentials credentials, TransportOptions? options = null)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Credentials = credentials,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true,
            };

            _handlers = new List<HttpMessageHandler>();

            for (int i = 0; i < options?.Handlers?.Length; i++)
            {
                _handlers.Add(options!.Handlers[i]);
            }

            _handlers.Add(httpClientHandler);


            for (int i = 1; i < _handlers.Count; i++)
            {
                DelegatingHandler? prevHandler = _handlers[i - 1] as DelegatingHandler;
                HttpMessageHandler currHandler = _handlers[i];

                if (prevHandler == null)
                {
                    throw new InvalidOperationException();
                }

                prevHandler.InnerHandler = currHandler;
            }

            HttpMessageHandler rootHandler = _handlers[0];
            
            _httpClient = new HttpClient(rootHandler)
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = options?.ReadTimeout ?? DefaultReadTimeout
            };
        }

        public async Task<string> Send(string message)
        {
            StringContent data = new StringContent(message, Encoding.UTF8, "application/soap+xml");

            using HttpResponseMessage response = await _httpClient.PostAsync("wsman", data).ConfigureAwait(false);

            try
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (HttpRequestException ex) when (response?.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new InvalidCredentialException("The specified credentials were rejected by the server", ex);
            }
            catch (HttpRequestException ex)
            {
                if (response != null)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    throw new TransportException((int)response.StatusCode, content);
                }

                throw new TransportException(500, string.Empty, ex);
            }
        }
    }
}
