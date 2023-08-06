using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class ItemData
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("hover")]
    public string Hover { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; } = 1;
    
    [JsonProperty("tier")]
    public int Tier { get; set; }
}