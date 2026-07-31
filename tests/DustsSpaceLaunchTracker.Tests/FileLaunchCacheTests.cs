using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;
using DustsSpaceLaunchTracker.Services.Data;

namespace DustsSpaceLaunchTracker.Tests;

public class FileLaunchCacheTests : IDisposable
{
    private readonly string _root;
    private readonly FileLaunchCache _cache;

    public FileLaunchCacheTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "DustsLaunchCacheTests", Guid.NewGuid().ToString("N"));
        _cache = new FileLaunchCache(_root);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch
        {
            // best-effort cleanup
        }
    }

    [Fact]
    public async Task SetAndGetPage_RoundTrips()
    {
        var page = new PagedResult<Launch>
        {
            Items =
            [
                new Launch { Id = "1", Name = "One" },
                new Launch { Id = "2", Name = "Two" }
            ],
            TotalCount = 2,
            Offset = 0,
            Limit = 15,
            HasNextPage = false
        };

        await _cache.SetPageAsync("upcoming|page0", page);
        var loaded = await _cache.GetPageAsync("upcoming|page0");

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded.Items.Count);
        Assert.Equal("One", loaded.Items[0].Name);
        Assert.Equal(2, loaded.TotalCount);
    }

    [Fact]
    public async Task SetAndGetDetail_RoundTrips()
    {
        var launch = new Launch { Id = "xyz", Name = "Detail launch" };
        await _cache.SetDetailAsync(launch);

        var loaded = await _cache.GetDetailAsync("xyz");
        Assert.NotNull(loaded);
        Assert.Equal("Detail launch", loaded.Name);
    }

    [Fact]
    public async Task GetPage_Missing_ReturnsNull()
    {
        var loaded = await _cache.GetPageAsync("missing-key");
        Assert.Null(loaded);
    }

    [Fact]
    public async Task Clear_RemovesEntries()
    {
        await _cache.SetDetailAsync(new Launch { Id = "z", Name = "Z" });
        await _cache.ClearAsync();

        Assert.Null(await _cache.GetDetailAsync("z"));
    }
}
