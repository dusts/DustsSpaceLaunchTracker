using System.Text.Json;
using DustsSpaceLaunchTracker.Models.Responses;
using DustsSpaceLaunchTracker.Services.Api;

namespace DustsSpaceLaunchTracker.Tests;

public class TheSpaceDevsJsonTests
{
    private static JsonSerializerOptions Options => TheSpaceDevsJson.CreateOptions();

    [Fact]
    public void Deserialize_PreviousPage_WithNullOrbitalCount_Succeeds()
    {
        var json = File.ReadAllText(Path.Combine("Fixtures", "previous_page_normal.json"));

        var page = JsonSerializer.Deserialize<LaunchListResponse>(json, Options);

        Assert.NotNull(page);
        Assert.Equal(2, page.Count);
        Assert.Equal(2, page.Results.Count);

        var falcon = page.Results[0];
        Assert.Equal("Falcon 9 Block 5 | Test Mission", falcon.Name);
        Assert.Equal(7350, falcon.OrbitalLaunchAttemptCount);
        Assert.Equal("Falcon 9 Block 5", falcon.Rocket?.Configuration?.FullName);
        Assert.Equal("Space Launch Complex 40", falcon.Pad?.Name);
        Assert.NotNull(falcon.Pad?.Latitude);

        var starship = page.Results[1];
        Assert.Equal("Starship | Flight 13", starship.Name);
        Assert.Null(starship.OrbitalLaunchAttemptCount);
        Assert.Equal(22, starship.LocationLaunchAttemptCount);
    }

    [Fact]
    public void Deserialize_MapsHoldAndFailReasonsWithoutUnderscore()
    {
        const string json = """
            {
              "count": 1,
              "next": null,
              "previous": null,
              "results": [
                {
                  "id": "x",
                  "name": "Test",
                  "holdreason": "weather",
                  "failreason": "engine",
                  "orbital_launch_attempt_count": 1,
                  "location_launch_attempt_count": 1
                }
              ]
            }
            """;

        var page = JsonSerializer.Deserialize<LaunchListResponse>(json, Options);

        Assert.NotNull(page);
        Assert.Equal("weather", page.Results[0].HoldReason);
        Assert.Equal("engine", page.Results[0].FailReason);
    }

    [Fact]
    public void Deserialize_PadLatLongFromStrings()
    {
        const string json = """
            {
              "id": 1,
              "name": "Pad",
              "latitude": "28.5",
              "longitude": "-80.5"
            }
            """;

        var pad = JsonSerializer.Deserialize<Models.Pad>(json, Options);

        Assert.NotNull(pad);
        Assert.Equal(28.5, pad.Latitude);
        Assert.Equal(-80.5, pad.Longitude);
    }
}
