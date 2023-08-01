using Newtonsoft.Json;

namespace BrotatoServer.Data.Game;

public class Weapon
{
    [JsonProperty("weapon_id")]
    public string WeaponId { get; set; }

    [JsonProperty("icon")]
    public string Icon { get; set; }

    [JsonProperty("tier")]
    public int Tier { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("effects_text")]
    public string EffectsText { get; set; }
}