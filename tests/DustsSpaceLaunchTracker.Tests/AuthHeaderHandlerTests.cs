using DustsSpaceLaunchTracker.Services.Api;
using System.Net;

namespace DustsSpaceLaunchTracker.Tests;

public class AuthHeaderHandlerTests
{
    private sealed class CaptureHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    [Fact]
    public async Task SendAsync_WithoutToken_DoesNotSetAuthorization()
    {
        var previous = Environment.GetEnvironmentVariable("DUSTS_LL_API_TOKEN");
        Environment.SetEnvironmentVariable("DUSTS_LL_API_TOKEN", null);
        try
        {
            var capture = new CaptureHandler();
            var handler = new AuthHeaderHandler { InnerHandler = capture };
            var client = new HttpClient(handler);

            await client.GetAsync("https://example.com/");

            Assert.NotNull(capture.LastRequest);
            Assert.Null(capture.LastRequest!.Headers.Authorization);
        }
        finally
        {
            Environment.SetEnvironmentVariable("DUSTS_LL_API_TOKEN", previous);
        }
    }

    [Fact]
    public async Task SendAsync_WithToken_SetsTokenScheme()
    {
        var previous = Environment.GetEnvironmentVariable("DUSTS_LL_API_TOKEN");
        Environment.SetEnvironmentVariable("DUSTS_LL_API_TOKEN", "secret-token");
        try
        {
            var capture = new CaptureHandler();
            var handler = new AuthHeaderHandler { InnerHandler = capture };
            var client = new HttpClient(handler);

            await client.GetAsync("https://example.com/");

            Assert.NotNull(capture.LastRequest?.Headers.Authorization);
            Assert.Equal("Token", capture.LastRequest!.Headers.Authorization!.Scheme);
            Assert.Equal("secret-token", capture.LastRequest.Headers.Authorization.Parameter);
        }
        finally
        {
            Environment.SetEnvironmentVariable("DUSTS_LL_API_TOKEN", previous);
        }
    }
}
