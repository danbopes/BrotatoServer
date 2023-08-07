using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public record CustomItemData
{
    [JsonProperty("name")]
    public string Name { get; init; } = "";
    
    [JsonProperty("tier")]
    public int Tier { get; init; } = 0;
    
    [JsonProperty("effects_text")]
    public string EffectsText { get; init; } = "";
    
    [JsonProperty("icon")]
    public string? Icon { get; init; } = "";
}