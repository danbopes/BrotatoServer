using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public record CustomData
{
    [JsonProperty("characters")]
    public Dictionary<string, CustomCharacterData>? Characters { get; init; }
    [JsonProperty("items")]
    public Dictionary<string, CustomItemData>? Items { get; init; }
    [JsonProperty("weapons")]
    public Dictionary<string, CustomItemData>? Weapons { get; init; }
}