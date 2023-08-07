using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public record CustomCharacterData
{
    [JsonProperty("name")]
    public string Name { get; init; } = "";
    
    [JsonProperty("effects_text")]
    public string EffectsText { get; init; } = "";
    
    [JsonProperty("icon")]
    public string? Icon { get; init; } = "";
}