using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrotatoServer.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong SteamId { get; set; }
    public string? TwitchUsername { get; set; }
    public ulong? TwitchId { get; set; }
    public bool JoinedChat { get; set; } = false;
    public Guid ApiKey { get; set; }
}