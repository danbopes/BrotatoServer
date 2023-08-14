using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BrotatoServer.Models.DB;

[Index(nameof(TwitchUsername))]
[Index(nameof(ApiKey), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong SteamId { get; set; }
    public string? TwitchUsername { get; set; }
    public ulong? TwitchId { get; set; }
    public string? TwitchAccessToken { get; set; }
    public bool JoinedChat { get; set; } = false;
    public Guid ApiKey { get; set; }
    public virtual UserSettings? Settings { get; set; }
    
    public string? CustomData { get; set; }
}