using Newtonsoft.Json;

namespace BrotatoServer.Data.Game;

public class Item
{
    [JsonProperty("icon")]
    public string Icon { get; set; }

    [JsonProperty("tier")]
    public int Tier { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("effects_text")]
    public string EffectsText { get; set; }

    [JsonProperty("tracking_text")]
    public string TrackingText { get; set; }
}
