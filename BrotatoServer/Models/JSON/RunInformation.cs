using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public record RunInformation
{
    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("ticks")]
    public long Ticks { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("streak_enabled", NullValueHandling = NullValueHandling.Ignore)]
    public bool? StreakEnabled { get; set; }

    [JsonProperty("mods", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, ModVersionInfo>? Mods { get; set; }

    [JsonProperty("run_data")]
    public RunData RunData { get; set; }
}
