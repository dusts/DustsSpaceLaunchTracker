using DustsSpaceLaunchTracker.Helpers;

namespace DustsSpaceLaunchTracker.Tests;

public class LaunchTimeFormatterTests
{
    [Fact]
    public void FormatUtc_Null_ReturnsTbd()
    {
        Assert.Equal("UTC: TBD", LaunchTimeFormatter.FormatUtc(null));
    }

    [Fact]
    public void FormatLocal_Null_ReturnsTbd()
    {
        Assert.Equal("NET: TBD", LaunchTimeFormatter.FormatLocal(null));
    }

    [Fact]
    public void FormatUtc_FormatsAsUtcClock()
    {
        var net = new DateTime(2026, 8, 1, 14, 30, 0, DateTimeKind.Utc);
        Assert.Equal("UTC: 2026-08-01 14:30", LaunchTimeFormatter.FormatUtc(net));
    }

    [Fact]
    public void FormatCountdown_Null_ReturnsTbd()
    {
        Assert.Equal("T‑ TBD", LaunchTimeFormatter.FormatCountdown(null, DateTime.UtcNow));
    }

    [Fact]
    public void FormatCountdown_FutureDays_UsesDayFormat()
    {
        var now = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);
        var net = now.AddDays(2).AddHours(3).AddMinutes(15);
        var text = LaunchTimeFormatter.FormatCountdown(net, now);
        Assert.Equal("T‑ 2d 03h 15m", text);
    }

    [Fact]
    public void FormatCountdown_FutureHours_UsesHourFormat()
    {
        var now = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);
        var net = now.AddHours(5).AddMinutes(4).AddSeconds(3);
        var text = LaunchTimeFormatter.FormatCountdown(net, now);
        Assert.Equal("T‑ 05h 04m 03s", text);
    }

    [Fact]
    public void FormatCountdown_Past_UsesTPlus()
    {
        var now = new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc);
        var net = now.AddMinutes(-10).AddSeconds(-5);
        var text = LaunchTimeFormatter.FormatCountdown(net, now);
        Assert.Equal("T+ 10m 05s", text);
    }

    [Fact]
    public void ToLocal_UnspecifiedKind_TreatedAsUtc()
    {
        var unspecified = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
        var local = LaunchTimeFormatter.ToLocal(unspecified);
        var expected = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc).ToLocalTime();
        Assert.Equal(expected, local);
    }
}
