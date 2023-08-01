using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AppearancesDisplayed
{
    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("display_priority")]
    public int DisplayPriority { get; set; }

    [JsonProperty("depth")]
    public int Depth { get; set; }

    [JsonProperty("sprite")]
    public string Sprite { get; set; }
}
