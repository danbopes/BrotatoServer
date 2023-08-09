using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class ItemData
{
    [JsonProperty("id")]
    public required string Id { get; set; }
    [JsonProperty("hover")]
    public required string Hover { get; set; }
    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("count")]
    public required int Count { get; set; } = 1;
    
    [JsonProperty("tier")]
    public required int Tier { get; set; }
}