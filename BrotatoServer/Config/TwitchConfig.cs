namespace BrotatoServer.Config;

public class TwitchConfig
{
    public required string BotName { get; init; }
    public required string BotOAuthToken { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}