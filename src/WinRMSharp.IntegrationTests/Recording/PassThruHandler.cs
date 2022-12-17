namespace WinRMSharp.IntegrationTests.Recording
{
    internal class PassThruHandler : SessionHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
