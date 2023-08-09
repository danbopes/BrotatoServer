using Newtonsoft.Json;

namespace BrotatoServer.Data.Game;

public class Weapon
{
    [JsonProperty("weapon_id")]
    public required string WeaponId { get; init; }

    [JsonProperty("icon")]
    public required string Icon { get; init; }

    [JsonProperty("tier")]
    public required int Tier { get; init; }

    [JsonProperty("name")]
    public required string Name { get; init; }

    [JsonProperty("effects_text")]
    public required string EffectsText { get; init; }
}