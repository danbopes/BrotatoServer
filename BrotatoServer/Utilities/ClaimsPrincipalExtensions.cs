using System.Security.Claims;

namespace BrotatoServer.Utilities;

public static class ClaimsPrincipalExtensions
{
    public static ulong GetSteamId(this ClaimsPrincipal claimsPrincipal)
    {
        //var id = claimsPrincipal.Identities.First(identity => identity.AuthenticationType == "ApiKey");
        var claim = claimsPrincipal.FindFirst("SteamId");

        if (claim is null)
        {
            claim = claimsPrincipal.Identities
                .FirstOrDefault(identity => identity.AuthenticationType == "Steam")?
                .FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            
            if (claim is null)
                throw new UnauthorizedAccessException("No claim SteamId");
        }

        if (!ulong.TryParse(claim.Value.Replace("https://steamcommunity.com/openid/id/", ""), out var steamId))
            throw new UnauthorizedAccessException("Steam Id was not a valid ulong");

        return steamId;
    }

    public static string GetTwitchName(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.Identities.FirstOrDefault(identity => identity.AuthenticationType == "Twitch")
            ?.Name;

        if (claim is null)
            throw new UnauthorizedAccessException("No claim Name for Twitch");

        return claim;
    }

    public static string GetTwitchRefreshToken(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.Identities
            .FirstOrDefault(identity => identity.AuthenticationType == "Twitch")?
            .FindFirst(c => c.Type == "TwitchRefreshToken")
            ?.Value;

        if (claim is null)
            throw new UnauthorizedAccessException("No claim Refresh Token for Twitch");

        return claim;
    }

    public static ulong GetTwitchId(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.Identities.FirstOrDefault(identity => identity.AuthenticationType == "Twitch")
            ?.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (claim is null)
            throw new UnauthorizedAccessException("No claim NameIdentifier for TwitchId");

        if (!ulong.TryParse(claim.Value, out var twitchId))
            throw new UnauthorizedAccessException("Twitch Id was not a valid ulong");

        return twitchId;
    }
}