using Newtonsoft.Json;

namespace BrotatoServer.Data.Game;

public class Item
{
    [JsonProperty("icon")]
    public required string Icon { get; init; }

    [JsonProperty("tier")]
    public required int Tier { get; init; }

    [JsonProperty("name")]
    public required string Name { get; init; }

    [JsonProperty("effects_text")]
    public required string EffectsText { get; init; }

    [JsonProperty("tracking_text")]
    public required string? TrackingText { get; init; }
}
