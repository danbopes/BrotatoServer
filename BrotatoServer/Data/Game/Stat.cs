using Newtonsoft.Json;

namespace BrotatoServer.Data.Game;

public class Stat
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("small_icon")]
    public string SmallIcon { get; set; }

    [JsonProperty("icon")]
    public string Icon { get; set; }
}
