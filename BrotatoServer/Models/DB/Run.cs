using Microsoft.EntityFrameworkCore;

namespace BrotatoServer.Models.DB;

[Index(nameof(UserId), nameof(Date), AllDescending = true)]
public class Run
{
    public Guid Id { get; set; }
    public ulong UserId { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool CurrentRotation { get; set; }
    public bool Won { get; set; }
    public string RunInformation { get; set; } = "";
    public string? CustomData { get; set; }
    public virtual User? User { get; set; }
    
    public string? TwitchClip { get; set; }
}