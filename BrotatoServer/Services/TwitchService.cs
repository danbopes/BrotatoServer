using BrotatoServer.Config;
using BrotatoServer.Data;
using BrotatoServer.Hubs;
using BrotatoServer.Models.DB;
using BrotatoServer.SearchEngine;
using BrotatoServer.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SearchEngine;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace BrotatoServer.Services;

public class TwitchService : BackgroundService
{
    private readonly ILogger<TwitchService> _log;
    private readonly BrotatoItemEngine _itemSearchEngine;
    private readonly BrotatoWeaponEngine _weaponSearchEngine;
    private readonly IServiceProvider _serviceProvider;
    private readonly TwitchClient _client;
    private readonly ConnectionCredentials _credentials;
    private readonly TwitchAPI _api;
    private readonly TwitchConfig _config;

#if DEBUG
    private const string PREFIX_CHAR = ";";
#else
    private const string PREFIX_CHAR = "!";
#endif


    public TwitchService(ILogger<TwitchService> log, IOptions<TwitchConfig> twitchConfig, ILoggerFactory loggerFactory, BrotatoItemEngine itemSearchEngine, BrotatoWeaponEngine weaponSearchEngine, IServiceProvider serviceProvider)
    {
        _config = twitchConfig.Value;
        _credentials = new ConnectionCredentials(_config.BotName, _config.BotOAuthToken);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        var customClient = new WebSocketClient(clientOptions);
        _client = new TwitchClient(customClient);
        _api = new TwitchAPI(loggerFactory)
        {
            Settings =
            {
                ClientId = _config.ClientId
            }
        };
        
        _log = log;
        _itemSearchEngine = itemSearchEngine;
        _weaponSearchEngine = weaponSearchEngine;
        _serviceProvider = serviceProvider;
    }

    private void Client_OnLog(object? sender, OnLogArgs e)
    {
        _log.LogTrace($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }

    private void Client_OnConnected(object? sender, OnConnectedArgs e)
    {
        _log.LogInformation("Connected to Twitch");
    }

    private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        if (string.IsNullOrEmpty(e.ChatMessage.Message))
            return;
        
        var words = e.ChatMessage.Message.Split(' ');

        switch (words[0])
        {
            case PREFIX_CHAR + "item":
            {
                HandleItemLookup(_itemSearchEngine, "item", e.ChatMessage.Channel, words);
                break;
            }
            case PREFIX_CHAR + "weapon":
            {
                HandleItemLookup(_weaponSearchEngine, "weapon", e.ChatMessage.Channel, words);
                break;
            }
            case PREFIX_CHAR + "tater":
                {
                    var runProvider = _serviceProvider.GetRequiredService<CurrentRunProvider>();
                    runProvider.Current.TryGetValue(e.ChatMessage.Channel, out var currentRun);

                    if (currentRun is not null)
                    {
                        var charName = currentRun.Character.Replace("character_", "");
                        var niceCharName = string.Join(' ', charName.Split('_').Select(word => word.UcFirst()));

                        _client.SendMessage(e.ChatMessage.Channel,
                            $"{niceCharName} - {e.ChatMessage.Channel.GetCurrentRunUrlForUser()}");
                    }
                    else
                    {
                        _client.SendMessage(e.ChatMessage.Channel, "There is no Brotato run currently in progress");
                    }
                    break;
                }
        }
    }

    private void HandleItemLookup(ISearchEngine engine, string engineType, string channel, string[] words)
    {
        if (words.Length < 2)
            return;

        var param = string.Join(' ', words.Skip(1));

        var card = engine.Find(param);

        var exactResults = card.Results.ToList();

        _client.SendMessage(channel,
            exactResults.Count == 0 ? $"No {engineType} found with that name" : exactResults.First().TextOutput);
    }

    public void JoinChat(string channel)
    {
        _client.JoinChannel(channel);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var channels = await userRepo.GetAllChatUsersAsync(stoppingToken).ToListAsync(stoppingToken);
        
        _client.Initialize(_credentials, channels);

        _client.OnLog += Client_OnLog;
        _client.OnMessageReceived += Client_OnMessageReceived;
        _client.OnConnected += Client_OnConnected;

        _client.Connect();
        await _itemSearchEngine.InitializeAsync();
    }

    public void SendMessage(string chan, string message)
    {
        _client.SendMessage(chan, message);
    }

    public async Task TryClipAsync(Guid runId, User user)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            
            var accessToken = await _api.Auth.RefreshAuthTokenAsync(user.TwitchAccessToken, _config.ClientSecret);
        
            var createdResult = await _api.Helix.Clips.CreateClipAsync(user.TwitchId.ToString(), accessToken.AccessToken);

            if (!createdResult.CreatedClips.Any())
            {
                _log.LogWarning("Unable to create clip for user {user}. No clips returned from create API", user.TwitchUsername);
                return;
            }
            
            var newClip = createdResult.CreatedClips.First();

            await Task.Delay(TimeSpan.FromSeconds(15));
            
            var clipResult = await _api.Helix.Clips.GetClipsAsync(new List<string>(new[] { newClip.Id }), accessToken: accessToken.AccessToken);
        
            if (!clipResult.Clips.Any())
            {
                _log.LogWarning("Unable to create clip for user {user}. No clips returned from get API", user.TwitchUsername);
                return;
            }
        
            var clip = clipResult.Clips.First();
        
            var db = scope.ServiceProvider.GetRequiredService<BrotatoServerContext>();

            await db.Run
                .Where(run => run.Id == runId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(r => r.TwitchClip, clip.EmbedUrl));
        
            _log.LogInformation("Created clip for user {user} with url {url}", user.TwitchUsername, clip.EmbedUrl);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Error creating clip for user {user}", user.TwitchUsername);
        }
    }
}
