using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class BurnChance
{
    [JsonProperty("chance")]
    public decimal Chance { get; set; }

    [JsonProperty("damage")]
    public int Damage { get; set; }

    [JsonProperty("duration")]
    public int Duration { get; set; }

    [JsonProperty("spread")]
    public int Spread { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }
}
