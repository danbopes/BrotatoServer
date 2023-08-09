using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public record RunInformation
{
    [JsonProperty("version")]
    public required string Version { get; set; }

    [JsonProperty("created")]
    public required long Created { get; set; }

    [JsonProperty("ticks")]
    public required long Ticks { get; set; }

    [JsonProperty("user_id")]
    public required ulong UserId { get; set; }

    [JsonProperty("streak_enabled", NullValueHandling = NullValueHandling.Ignore)]
    public required bool? StreakEnabled { get; set; }

    [JsonProperty("mods", NullValueHandling = NullValueHandling.Ignore)]
    public required Dictionary<string, ModVersionInfo>? Mods { get; set; }

    [JsonProperty("run_data")]
    public required RunData RunData { get; set; }
}
