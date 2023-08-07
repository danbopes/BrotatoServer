using System.Net;
using System.Security.Claims;
using System.Text;
using BrotatoServer;
using BrotatoServer.Data;
using BrotatoServer.Hubs;
using BrotatoServer.SearchEngine;
using BrotatoServer.Services;
using BrotatoServer.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Steamworks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("db/data_protection_keys"));

builder.Services.AddRazorPages();
builder.Services.AddDbContext<BrotatoServerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BrotatoServerContext") ?? throw new InvalidOperationException("Connection string 'BrotatoServerContext' not found.")));

builder.Services.AddServerSideBlazor();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.1.0.0"), 16));
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddHealthChecks();

//builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

builder.Services
    .AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.LoginPath = "/setup";
    })
    .AddSteam(options =>
    {
        options.SignInScheme = "cookie";
        options.CallbackPath = "/oauth/steam-callback";
        options.ApplicationKey = "EBDE5FDBBABC696473B3B4D7AD59AE60";

        options.Events.OnAuthenticated = async ctx =>
        {
            var schemeProvider = ctx.HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            var defaultAuthenticate = await schemeProvider.GetDefaultAuthenticateSchemeAsync();
            var authType = ctx.Ticket.Principal!.Identity!.AuthenticationType;

            if (defaultAuthenticate != null)
            {
                var result = await ctx.HttpContext.AuthenticateAsync(defaultAuthenticate.Name);

                if (result?.Principal != null)
                {
                    var clone = result.Principal.Clone();

                    ctx.Ticket.Principal!.AddIdentities(clone.Identities.Where(identity => identity.AuthenticationType != authType));
                }
            }
        };
    })
    .AddTwitch(options =>
    {
        options.SignInScheme = "cookie";
        options.CallbackPath = "/oauth/twitch-callback";
        options.ClientId = AppConstants.TWITCH_CLIENT_ID;
        options.ClientSecret = AppConstants.TWITCH_CLIENT_SECRET;
        options.Scope.Add("clips:edit");

        options.Events.OnCreatingTicket = async ctx =>
        {
            var schemeProvider = ctx.HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            var defaultAuthenticate = await schemeProvider.GetDefaultAuthenticateSchemeAsync();
            var authType = ctx.Principal!.Identity!.AuthenticationType;

            ctx.Principal!.Identities.First().AddClaim(new Claim("TwitchRefreshToken", ctx.RefreshToken!));
            
            if (defaultAuthenticate != null)
            {
                var result = await ctx.HttpContext.AuthenticateAsync(defaultAuthenticate.Name);

                if (result?.Principal != null)
                {
                    var clone = result.Principal.Clone();

                    ctx.Principal!.AddIdentities(clone.Identities.Where(identity => identity.AuthenticationType != authType));
                }
            }
        };
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationSchemeHandler>("ApiKey", o => { });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy(AuthPolicies.TWITCH_USER, p => p.RequireAssertion(ctx => ctx.User.Identities.Any(identity => identity.AuthenticationType == "Twitch")));
    o.AddPolicy(AuthPolicies.FULLY_AUTHED_USER,
        p => p.RequireAssertion(ctx => 
            ctx.User.Identities.Any(identity => identity.AuthenticationType == "Twitch") &&
            ctx.User.Identities.Any(identity => identity.AuthenticationType == "Steam")));
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services
    .AddScoped<IRunRepository, RunRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddSingleton<CurrentRunProvider>();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddSignalR();
builder.Services.AddRequestDecompression();

builder.Services
    .AddSingleton<TwitchService>()
    .AddHostedService(sp => sp.GetRequiredService<TwitchService>());
builder.Services.AddSingleton<BrotatoItemEngine>();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});
builder.Services.AddScoped<SessionStorage>();

var app = builder.Build();

app.UseRequestDecompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/*
app.Use((context, next) =>
{
    // Check if the request is for an API endpoint
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        // Check if the "ApiKey" header exists and its value matches the expected API key
        if (!context.Request.Headers.TryGetValue("x-api-key", out var headerValue) || headerValue != API_KEY)
        {
            // If the API key is missing or doesn't match, return 401 Unauthorized
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    }

    // If the API key is valid or the request is not for an API endpoint, continue to the next middleware
    return next();
});
*/
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<RunsHub>("/runsHub");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var log = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var dbContext = services.GetRequiredService<BrotatoServerContext>(); // Replace 'YourDbContext' with your actual DbContext class.
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Handle the exception as needed (e.g., log or display an error message).
        log.LogCritical(ex, "Error occurred while applying migrations: ");
        return;
    }
    
#if DEBUG
    const int APP_ID = 480;
#else
    const int APP_ID = 1942280;
#endif
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

try
{
    app.Run();
}
finally
{
    SteamServer.Shutdown();
}
