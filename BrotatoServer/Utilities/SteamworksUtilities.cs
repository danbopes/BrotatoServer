using Azure;
using Steamworks;

namespace BrotatoServer.Utilities;

public static class SteamworksUtilities
{
    public static async Task<bool> AuthenticateSession(byte[] token, SteamId steamId)
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
