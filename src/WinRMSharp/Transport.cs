using System.Net;
using System.Security.Authentication;
using System.Text;
using WinRMSharp.Exceptions;

namespace WinRMSharp
{
    /// <summary>
    /// Options used to configure a <see cref="Transport"/> instance.
    /// </summary>
    public class TransportOptions
    {
        /// <summary>
        /// Maximum timeout to wait before an HTTP connect/read times out.
        /// </summary>
        public TimeSpan? ReadTimeout { get; set; }
    }

    public class Transport : ITransport
    {
        private static readonly TimeSpan DefaultReadTimeout = TimeSpan.FromSeconds(30);

        private readonly HttpClient _httpClient;

        /// <inheritdoc cref="ITransport.Transport" />
        public TimeSpan ReadTimeout => _httpClient.Timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transport"/> class.
        /// </summary>
        /// <param name="baseUrl">Base url of the destination host.</param>
        /// <param name="credentials">Credentials used to secure communication to the destination host.</param>
        /// <param name="options">Transport options.</param>
        public Transport(string baseUrl, ICredentials credentials, TransportOptions? options = null)
            : this(baseUrl, GenerateSecureHandler(credentials), options)
        {
        }

        internal Transport(string baseUrl, DelegatingHandler wrapper, ICredentials credentials, TransportOptions? options = null)
            : this(baseUrl, GenerateWrappedHandler(wrapper, GenerateSecureHandler(credentials)), options)
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

        /// <inheritdoc cref="ITransport.Send" />
        public async Task<string> Send(string message)
        {
            StringContent data = new StringContent(message, Encoding.UTF8, "application/soap+xml");

            //OnMessage?.Invoke($"Sending message: '{message}'");

            using HttpResponseMessage response = await _httpClient.PostAsync("wsman", data).ConfigureAwait(false);

            //OnMessage?.Invoke($"Receiving message: '{await response.Content.ReadAsStringAsync()}'");

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

                throw new TransportException(statusCode, content, ex);
            }
        }

        private static DelegatingHandler GenerateWrappedHandler(DelegatingHandler wrapper, HttpMessageHandler inner)
        {
            wrapper.InnerHandler = inner;
            return wrapper;
        }

        private static HttpMessageHandler GenerateSecureHandler(ICredentials credentials)
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
