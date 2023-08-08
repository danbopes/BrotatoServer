using System.Net;
using System.Text;
using Steamworks;

namespace BrotatoServer.Services;

public class SteamworksService : ISteamworksService
{
#if DEBUG
    private const int APP_ID = 480;
#else
    private const int APP_ID = 1942280;
#endif
    public SteamworksService(ILogger<SteamworksService> log)
    {
        File.WriteAllText("steam_appid.txt", APP_ID.ToString(), Encoding.ASCII);
        log.LogInformation("Initializing Steam - Init");
        SteamServer.Init(APP_ID, new SteamServerInit
        {
        
            DedicatedServer = true,
            IpAddress = IPAddress.Loopback,
            VersionString = "0.0.1"
        });
        log.LogInformation("Initializing Steam - Logging On");
        SteamServer.LogOnAnonymous();
    }
    public async Task<bool> AuthenticateSessionAsync(byte[] token, SteamId steamId)
    {
        var tcs = new TaskCompletionSource<bool>();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        cts.Token.Register(() => tcs.TrySetCanceled());
        cts.CancelAfter(TimeSpan.FromSeconds(5));

        void ValidateTicketResponse(SteamId resSteamId, SteamId resOwnerId, AuthResponse response)
        {
            // Since we're using a callbacks that may be shared for requests, we need to make sure we're
            // only handling the response for the request we care about.
            if (resSteamId != steamId)
                return; 

            if (resSteamId != resOwnerId || response != AuthResponse.OK)
            {
                tcs!.TrySetResult(false);
                return;
            }

            tcs!.TrySetResult(true);
        };

        SteamServer.OnValidateAuthTicketResponse += ValidateTicketResponse;

        var res = SteamServer.BeginAuthSession(token, steamId);

        try
        {
            if (!res)
                return false;

            try
            {
                return await tcs.Task;
            }
            finally
            {
                SteamServer.EndSession(steamId);
            }
        }
        finally
        {
            SteamUser.OnValidateAuthTicketResponse -= ValidateTicketResponse;
        }
    }
}
