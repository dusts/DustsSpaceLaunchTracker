using CommunityToolkit.Maui;
using DustsSpaceLaunchTracker.Services;
using DustsSpaceLaunchTracker.Services.Api;
using DustsSpaceLaunchTracker.ViewModels;
using DustsSpaceLaunchTracker.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;// for DelayBackoffType
using System.Net;

namespace DustsSpaceLaunchTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ── Refit + new Resilience pipeline ───────────────────────────────────────
            builder.Services.AddRefitClient<ITheSpaceDevsApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("https://ll.thespacedevs.com/2.2.0/");
                c.Timeout = TimeSpan.FromSeconds(25);
            })
            .AddStandardResilienceHandler(options =>
            {
                // Retry: 3 attempts, exponential backoff starting at 2s (2→4→8s), handle transients + 429 + 5xx
                options.Retry = new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),                    // base delay
                    BackoffType = DelayBackoffType.Exponential,         // exponential: 2s → 4s → 8s
                    UseJitter = true,                                   // optional: adds randomness to prevent thundering herd
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<HttpRequestException>()                 // network-level errors
                        .HandleResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)  // 429 rate limit
                        .HandleResult(r => (int)r.StatusCode >= 500)    // 5xx server errors
                };

                // Circuit Breaker: break after 50% failures in 10 requests, break for 30s
                options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    MinimumThroughput = 10,                             // at least 10 calls before evaluating
                    BreakDuration = TimeSpan.FromSeconds(120),
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<HttpRequestException>()
                        .HandleResult(r => (int)r.StatusCode >= 500)
                };

                // Timeout: per-request timeout (attempt timeout)
                options.AttemptTimeout = new HttpTimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(15)
                };

                // Optional: total pipeline timeout (whole operation including retries)
                options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };

                // Optional: rate limiter if API is very strict – but skip for now unless you hit hard limits
                // options.RateLimiter = new ...
            });

            // Services & ViewModels
            builder.Services.AddSingleton<LaunchService>();
            builder.Services.AddTransient<UpcomingLaunchesViewModel>();
            builder.Services.AddTransient<UpcomingLaunchesPage>();

            return builder.Build();
        }
    }
}
