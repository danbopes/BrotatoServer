namespace BrotatoServer;

public static class AuthPolicies
{
    /// <summary>
    /// A policy which indicates the logged in user
    /// has authenticated properly with Twitch
    /// </summary>
    public const string TWITCH_USER = "TwitchUser";

    public const string STEAM_USER = "SteamUser";

    public const string FULLY_AUTHED_USER = "FullyAuthedUser";
}