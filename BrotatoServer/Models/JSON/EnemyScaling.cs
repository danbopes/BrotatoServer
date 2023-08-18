using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class EnemyScaling
{
    [JsonProperty("health")]
    public decimal Health { get; set; }

    [JsonProperty("damage")]
    public decimal Damage { get; set; }

    [JsonProperty("speed")]
    public decimal Speed { get; set; }
}
