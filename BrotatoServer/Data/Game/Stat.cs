using Newtonsoft.Json;

namespace BrotatoServer.Data.Game;

public class Stat
{
    [JsonProperty("name")]
    public required string? Name { get; init; }

    [JsonProperty("small_icon")]
    public required string SmallIcon { get; init; }

    [JsonProperty("icon")]
    public required string Icon { get; init; }
}
