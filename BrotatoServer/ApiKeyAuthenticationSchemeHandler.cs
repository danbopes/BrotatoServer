
using BrotatoServer.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BrotatoServer;

public class ApiKeyAuthenticationSchemeHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string API_KEY_HEADER = "X-API-KEY";
    public ApiKeyAuthenticationSchemeHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
            return AuthenticateResult.Fail("Api Key was not provided.");

        if (!Guid.TryParse(extractedApiKey[0], out var apiKeyGuid))
            return AuthenticateResult.Fail("Api Key was invalid.");

        var db = Context.RequestServices.GetRequiredService<BrotatoServerContext>();

        var user = await db.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKeyGuid);

        if (user is null)
            return AuthenticateResult.Fail("That Api Key was not found.");
        
        Context.Items.Add("User", user);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] {
                new Claim("SteamId", user.SteamId.ToString()),
                new Claim("TwitchUsername", user.TwitchUsername ?? "")
            },
            Scheme.Name));

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}