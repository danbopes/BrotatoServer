﻿using BrotatoServer.Data;
using BrotatoServer.Hubs;
using BrotatoServer.Models.DB;
using BrotatoServer.SearchEngine;
using BrotatoServer.Utilities;
using Microsoft.EntityFrameworkCore;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly TwitchClient _client;
    private readonly ConnectionCredentials _credentials;

#if DEBUG
    private const string PREFIX_CHAR = ";";
#else
    private const string PREFIX_CHAR = "!";
#endif


    public TwitchService(ILogger<TwitchService> log, BrotatoItemEngine itemSearchEngine, IServiceProvider serviceProvider)
    {
        _credentials = new ConnectionCredentials(AppConstants.BOT_NAME, "oauth:gx244vysokmuqcunclv8fovswigefo");
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        var customClient = new WebSocketClient(clientOptions);
        _client = new TwitchClient(customClient);
        _log = log;
        _itemSearchEngine = itemSearchEngine;
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

    private async void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        if (string.IsNullOrEmpty(e.ChatMessage.Message))
            return;
        
        var words = e.ChatMessage.Message.Split(' ');

        switch (words[0])
        {
            case PREFIX_CHAR + "item":
                {
                    if (words.Length < 2)
                        return;

                    var param = string.Join(' ', words.Skip(1));

                    var card = await _itemSearchEngine.FindAsync(param);

                    var exactResults = card.Results.ToList();

                    if (exactResults.Count == 0)
                    {
                        _client.SendMessage(e.ChatMessage.Channel, "No item found with that name");
                    }
                    else
                    {
                        _client.SendMessage(e.ChatMessage.Channel, exactResults.First().TextOutput);
                    }
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
            var twitchApi = new TwitchAPI(scope.ServiceProvider.GetRequiredService<ILoggerFactory>())
            {
                Settings =
                {
                    ClientId = AppConstants.TWITCH_CLIENT_ID
                }
            };

            var accessToken = await twitchApi.Auth.RefreshAuthTokenAsync(user.TwitchAccessToken, AppConstants.TWITCH_CLIENT_SECRET);
        
            twitchApi.Settings.AccessToken = accessToken.AccessToken;

            var createdResult = await twitchApi.Helix.Clips.CreateClipAsync(user.TwitchId.ToString());

            if (!createdResult.CreatedClips.Any())
            {
                _log.LogWarning("Unable to create clip for user {user}. No clips returned from create API", user.TwitchUsername);
                return;
            }
            
            var newClip = createdResult.CreatedClips.First();

            await Task.Delay(TimeSpan.FromSeconds(15));
            
            var clipResult = await twitchApi.Helix.Clips.GetClipsAsync(new List<string>(new[] { newClip.Id }));
        
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