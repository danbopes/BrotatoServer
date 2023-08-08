using Steamworks;

namespace BrotatoServer.Services;

public interface ISteamworksService
{
    Task<bool> AuthenticateSessionAsync(byte[] token, SteamId steamId);
}