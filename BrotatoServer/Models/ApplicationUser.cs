using Microsoft.AspNetCore.Identity;

namespace BrotatoServer.Models;


public class ApplicationUser : IdentityUser
{
    public string? TwitchUsername { get; set; }
    public string? SteamId { get; set; }
}