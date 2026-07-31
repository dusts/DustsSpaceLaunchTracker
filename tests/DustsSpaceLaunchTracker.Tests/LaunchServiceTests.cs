using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;
using DustsSpaceLaunchTracker.Services;
using DustsSpaceLaunchTracker.Services.Api;
using DustsSpaceLaunchTracker.Services.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace DustsSpaceLaunchTracker.Tests;

public class LaunchServiceTests
{
    private readonly Mock<ITheSpaceDevsApi> _api = new();
    private readonly Mock<ILaunchCache> _cache = new();
    private readonly LaunchService _sut;

    public LaunchServiceTests()
    {
        _sut = new LaunchService(_api.Object, _cache.Object, NullLogger<LaunchService>.Instance);
    }

    [Fact]
    public async Task GetUpcomingPageAsync_MapsApiResponseAndCaches()
    {
        var launches = new List<Launch>
        {
            new() { Id = "1", Name = "A" },
            new() { Id = "2", Name = "B" }
        };

        _api.Setup(a => a.GetUpcomingLaunchesAsync(
                15, 0, It.IsAny<string>(), null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LaunchListResponse
            {
                Count = 100,
                Next = "https://example.com/next",
                Results = launches
            });

        PagedResult<Launch>? cached = null;
        _cache.Setup(c => c.SetPageAsync(It.IsAny<string>(), It.IsAny<PagedResult<Launch>>(), It.IsAny<CancellationToken>()))
            .Callback<string, PagedResult<Launch>, CancellationToken>((_, page, _) => cached = page)
            .Returns(Task.CompletedTask);

        var page = await _sut.GetUpcomingPageAsync(limit: 15, offset: 0);

        Assert.Equal(2, page.Items.Count);
        Assert.Equal(100, page.TotalCount);
        Assert.True(page.HasNextPage);
        Assert.Equal(2, page.NextOffset);
        Assert.NotNull(cached);
        Assert.Equal(2, cached!.Items.Count);
    }

    [Fact]
    public async Task GetUpcomingPageAsync_OnApiFailure_ReturnsCache()
    {
        _api.Setup(a => a.GetUpcomingLaunchesAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("offline"));

        var stale = new PagedResult<Launch>
        {
            Items = [new Launch { Id = "cached", Name = "From cache" }],
            TotalCount = 1,
            Offset = 0,
            Limit = 15,
            HasNextPage = false
        };

        _cache.Setup(c => c.GetPageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stale);

        var page = await _sut.GetUpcomingPageAsync(limit: 15, offset: 0);

        Assert.Single(page.Items);
        Assert.Equal("cached", page.Items[0].Id);
    }

    [Fact]
    public async Task GetUpcomingPageAsync_OnApiFailureWithoutCache_Throws()
    {
        _api.Setup(a => a.GetUpcomingLaunchesAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("offline"));

        _cache.Setup(c => c.GetPageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagedResult<Launch>?)null);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _sut.GetUpcomingPageAsync(limit: 15, offset: 0));
    }

    [Fact]
    public async Task GetLaunchDetailAsync_CachesSuccessfulFetch()
    {
        var launch = new Launch { Id = "abc", Name = "Detail" };
        _api.Setup(a => a.GetLaunchDetailAsync("abc", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(launch);
        _cache.Setup(c => c.GetDetailAsync("abc", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Launch?)null);

        var result = await _sut.GetLaunchDetailAsync("abc");

        Assert.Equal("Detail", result.Name);
        _cache.Verify(c => c.SetDetailAsync(It.Is<Launch>(l => l.Id == "abc"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPreviousPageAsync_UsesPreviousApi()
    {
        _api.Setup(a => a.GetPreviousLaunchesAsync(
                10, 20, It.IsAny<string>(), "star", 3, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LaunchListResponse
            {
                Count = 50,
                Results = [new Launch { Id = "p1", Name = "Prev" }]
            });

        _cache.Setup(c => c.SetPageAsync(It.IsAny<string>(), It.IsAny<PagedResult<Launch>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var page = await _sut.GetPreviousPageAsync(
            limit: 10, offset: 20, search: "star", statusId: 3);

        Assert.Single(page.Items);
        Assert.Equal("p1", page.Items[0].Id);
        _api.VerifyAll();
    }
}
