using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AppearancesDisplayed
{
    [JsonProperty("position")]
    public required int Position { get; set; }

    [JsonProperty("display_priority")]
    public required int DisplayPriority { get; set; }

    [JsonProperty("depth")]
    public required int Depth { get; set; }

    [JsonProperty("sprite")]
    public required string? Sprite { get; set; }
}
