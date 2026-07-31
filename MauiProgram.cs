using CommunityToolkit.Maui;
using DustsSpaceLaunchTracker.Configuration;
using DustsSpaceLaunchTracker.Services;
using DustsSpaceLaunchTracker.Services.Api;
using DustsSpaceLaunchTracker.Services.Data;
using DustsSpaceLaunchTracker.ViewModels;
using DustsSpaceLaunchTracker.Views;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;
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

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(TheSpaceDevsJson.CreateOptions())
            };

            builder.Services.AddTransient<AuthHeaderHandler>();

            builder.Services.AddRefitClient<ITheSpaceDevsApi>(refitSettings)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
                    c.Timeout = TimeSpan.FromSeconds(25);
                    c.DefaultRequestHeaders.UserAgent.ParseAdd("DustsSpaceLaunchTracker/1.0");
                })
                .AddHttpMessageHandler<AuthHeaderHandler>()
                .AddStandardResilienceHandler(options =>
                {
                    options.Retry = new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(2),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                            .Handle<HttpRequestException>()
                            .HandleResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                            .HandleResult(r => (int)r.StatusCode >= 500)
                    };

                    options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
                    {
                        FailureRatio = 0.5,
                        MinimumThroughput = 10,
                        BreakDuration = TimeSpan.FromSeconds(120),
                        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                            .Handle<HttpRequestException>()
                            .HandleResult(r => (int)r.StatusCode >= 500)
                    };

                    options.AttemptTimeout = new HttpTimeoutStrategyOptions
                    {
                        Timeout = TimeSpan.FromSeconds(15)
                    };

                    options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
                    {
                        Timeout = TimeSpan.FromSeconds(60)
                    };
                });

            // Data & services
            builder.Services.AddSingleton<ILaunchCache, FileLaunchCache>();
            builder.Services.AddSingleton<ILaunchService, LaunchService>();

            // Shell + pages + VMs (point 1: constructor DI)
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<UpcomingLaunchesViewModel>();
            builder.Services.AddTransient<PreviousLaunchesViewModel>();
            builder.Services.AddTransient<LaunchDetailViewModel>();
            builder.Services.AddTransient<UpcomingLaunchesPage>();
            builder.Services.AddTransient<PreviousLaunchesPage>();
            builder.Services.AddTransient<LaunchDetailPage>();

            return builder.Build();
        }
    }
}
