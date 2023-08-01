using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class RunInformation
{
    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserId { get; set; }

    [JsonProperty("mods", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, ModVersionInfo>? Mods { get; set; }

    [JsonProperty("run_data")]
    public RunData RunData { get; set; }
}
