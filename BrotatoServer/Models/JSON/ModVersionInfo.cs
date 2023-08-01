using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class ModVersionInfo
{
    [JsonProperty("version")]
    public string Version { get; set; }
}


