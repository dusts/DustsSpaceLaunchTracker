using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;
using DustsSpaceLaunchTracker.ViewModels;

namespace DustsSpaceLaunchTracker.Tests;

public class PagedResultAndStatusFilterTests
{
    [Fact]
    public void NextOffset_IsOffsetPlusItemCount()
    {
        var page = new PagedResult<Launch>
        {
            Items = [new Launch(), new Launch(), new Launch()],
            Offset = 15,
            Limit = 15,
            TotalCount = 100,
            HasNextPage = true
        };

        Assert.Equal(18, page.NextOffset);
    }

    [Fact]
    public void StatusFilterOption_All_HasNullId()
    {
        Assert.Null(StatusFilterOption.All.Id);
        Assert.Equal("All statuses", StatusFilterOption.All.Name);
    }

    [Fact]
    public void StatusFilterOption_AllOptions_StartsWithAll()
    {
        Assert.Same(StatusFilterOption.All, StatusFilterOption.AllOptions[0]);
        Assert.Contains(StatusFilterOption.AllOptions, o => o.Id == 3 && o.Name == "Success");
    }

    [Fact]
    public void ApiRoutes_IncludeVersionPrefix()
    {
        Assert.StartsWith("/2.2.0/", Configuration.ApiRoutes.UpcomingLaunches);
        Assert.StartsWith("/2.2.0/", Configuration.ApiRoutes.PreviousLaunches);
        Assert.Contains("{id}", Configuration.ApiRoutes.LaunchDetail);
    }

    [Fact]
    public void AppConfig_ListAndDetailModes()
    {
        Assert.Equal("normal", Configuration.AppConfig.ListMode);
        Assert.Equal("detailed", Configuration.AppConfig.DetailMode);
        Assert.True(Configuration.AppConfig.PageSize > 0);
    }
}
