using BrotatoServer.Models.JSON;

namespace BrotatoServer.Models.DB;

public class FullRun
{
    public Guid Id { get; set; }
    public string? TwitchClip { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool CurrentRotation { get; set; }
    public RunData RunData { get; set; }
    public CustomData? CustomData { get; set; }
}