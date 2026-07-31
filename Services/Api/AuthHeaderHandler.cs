using DustsSpaceLaunchTracker.Configuration;
using System.Net.Http.Headers;

namespace DustsSpaceLaunchTracker.Services.Api
{
    /// <summary>Adds optional API token for higher rate limits (point 6).</summary>
    public sealed class AuthHeaderHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = AppConfig.ApiToken;
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Token", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
