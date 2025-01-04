using System.Security.Claims;
using BrotatoServer;
using BrotatoServer.Config;
using BrotatoServer.Data;
using BrotatoServer.Hubs;
using BrotatoServer.SearchEngine;
using BrotatoServer.Services;
using BrotatoServer.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Steamworks;
using IPAddress = System.Net.IPAddress;

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
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("192.168.0.0"), 16));
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


var twitchConfigSection = builder.Configuration.GetSection("Twitch");
var twitchConfig = twitchConfigSection.Get<TwitchConfig>() ?? throw new InvalidOperationException("Twitch config not found.");
var steamConfigSection = builder.Configuration.GetSection("Steam");
var steamConfig = steamConfigSection.Get<SteamConfig>() ?? throw new InvalidOperationException("Steam config not found.");

builder.Services
    .Configure<SteamConfig>(steamConfigSection)
    .Configure<TwitchConfig>(twitchConfigSection);

builder.Services.AddHealthChecks();

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
        options.ApplicationKey = steamConfig.AppKey;

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
        options.ClientId = twitchConfig.ClientId;
        options.ClientSecret = twitchConfig.ClientSecret;
        options.Scope.Add("clips:edit");
        options.Scope.Add("channel:bot");

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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthPolicies.TWITCH_USER, p => p.RequireAssertion(ctx => ctx.User.Identities.Any(identity => identity.AuthenticationType == "Twitch")))
    .AddPolicy(AuthPolicies.FULLY_AUTHED_USER, p => p.RequireAssertion(ctx => 
            ctx.User.Identities.Any(identity => identity.AuthenticationType == "Twitch") &&
            ctx.User.Identities.Any(identity => identity.AuthenticationType == "Steam")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services
    .AddScoped<IRunRepository, RunRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddSingleton<ISteamworksService, SteamworksService>()
    .AddSingleton<CurrentRunProvider>();

builder.Services.AddMetricServer(o =>
{
    o.Port = 1234;
});

builder.Services.AddSingleton<CircuitHandler>(new CircuitHandlerService());

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        var loggerFactory = LoggerFactory
            .Create(logBuilder =>
            {
                logBuilder.ClearProviders();
                logBuilder.AddConsole();
            });
        
        var logger = loggerFactory
            .CreateLogger<Program>();
        
        options.SerializerSettings.Error = (_, args) =>
        {
            logger.LogWarning("Json Deserialization Exception: {Exception}\n {CurrentObject}", args.ErrorContext.Error.Message, args.CurrentObject);
        };
    });

builder.Services.AddSignalR();
builder.Services.AddRequestDecompression();

builder.Services
    .AddSingleton<TwitchService>()
    .AddHostedService(sp => sp.GetRequiredService<TwitchService>());
builder.Services
    .AddSingleton<BrotatoItemEngine>()
    .AddSingleton<BrotatoWeaponEngine>();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          ["application/octet-stream"]);
});

builder.Services.AddHttpClient();

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

app.UseStaticFiles();

app.UseRouting();

app.UseHttpMetrics();

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
        var dbContext = services.GetRequiredService<BrotatoServerContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        log.LogCritical(ex, "Error occurred while applying migrations");
        return;
    }

    // Initialize steamworks service (Just need to create force new instance)
    services.GetRequiredService<ISteamworksService>();
}

try
{
    app.Run();
}
finally
{
    SteamServer.Shutdown();
}
