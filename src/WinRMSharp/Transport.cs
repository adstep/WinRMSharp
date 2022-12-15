using System.Net;
using System.Security.Authentication;
using System.Text;
using WinRMSharp.Exceptions;

namespace WinRMSharp
{
    public class TransportOptions
    {
        public TimeSpan? ReadTimeout { get; set; }
    }

    public class Transport : ITransport
    {
        private static readonly TimeSpan DefaultReadTimeout = TimeSpan.FromSeconds(30);

        private readonly HttpClient _httpClient;

        public Transport(string baseUrl, ICredentials credentials, TransportOptions? options = null)
            : this(baseUrl, GenerateSecureHandler(credentials), options)
        {
        }

        internal Transport(string baseUrl, HttpMessageHandler messageHandler, TransportOptions? options = null)
        {
            _httpClient = new HttpClient(messageHandler)
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
                string content = string.Empty;
                int statusCode = 500;

                if (response != null)
                {
                    content = await response.Content.ReadAsStringAsync();
                    statusCode = (int)response.StatusCode;
                }

                throw new TransportException(500, content, ex);
            }
        }

        private static HttpClientHandler GenerateSecureHandler(ICredentials credentials)
        {
            return new HttpClientHandler()
            {
                Credentials = credentials,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true,
            };
        }
    }
}
