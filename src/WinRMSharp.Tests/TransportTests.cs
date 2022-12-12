using Moq;
using Moq.Protected;
using System.Net;
using System.Security.Authentication;
using WinRMSharp.Exceptions;
using Xunit;

namespace WinRMSharp.Tests
{
    public class TransportTests
    {
        [Fact]
        public async Task AuthorizationFailure()
        {
            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            HttpMessageHandler httpMessageHandler = mockHttpMessageHandler.Object;

            Transport transport = new Transport("https://localhost:5986", httpMessageHandler);

            await Assert.ThrowsAsync<InvalidCredentialException>(async () => await transport.Send(string.Empty));
        }

        [Fact]
        public async Task TransportFailure()
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string errorContent = "example-error-content";

            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode) { Content = new StringContent(errorContent)});
            HttpMessageHandler httpMessageHandler = mockHttpMessageHandler.Object;

            Transport transport = new Transport("https://localhost:5986", httpMessageHandler);

            TransportException transportException = await Assert.ThrowsAsync<TransportException>(async () => await transport.Send(string.Empty));

            Assert.Equal((int)statusCode, transportException.Code);
            Assert.Equal(errorContent, transportException.Content);
        }
    }
}
