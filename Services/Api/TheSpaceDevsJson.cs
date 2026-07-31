using System.Text.Json;
using System.Text.Json.Serialization;

namespace DustsSpaceLaunchTracker.Services.Api
{
    /// <summary>
    /// Shared JSON options for The Space Devs Launch Library API
    /// (snake_case payloads with a few irregular property names).
    /// </summary>
    public static class TheSpaceDevsJson
    {
        public static JsonSerializerOptions CreateOptions() => new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // founding_year, latitude, longitude, etc. often arrive as strings
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };
    }
}
