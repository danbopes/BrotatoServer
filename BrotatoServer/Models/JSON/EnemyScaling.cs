using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class EnemyScaling
{
    [JsonProperty("health")]
    public int Health { get; set; }

    [JsonProperty("damage")]
    public int Damage { get; set; }

    [JsonProperty("speed")]
    public int Speed { get; set; }
}
