using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class ModVersionInfo
{
    [JsonProperty("version")]
    public required string Version { get; set; }
}


