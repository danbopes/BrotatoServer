using BrotatoServer.Hubs;
using BrotatoServer.SearchEngine;
using BrotatoServer.Utilities;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace BrotatoServer.Services;

public class TwitchChatService : BackgroundService
{
    private readonly ILogger<TwitchChatService> _log;
    private readonly BrotatoItemEngine _itemSearchEngine;
    private readonly IServiceProvider _serviceProvider;
    private readonly TwitchClient _client;
    private readonly ConnectionCredentials _credentials;

#if DEBUG
    private const string PREFIX_CHAR = ";";
#else
    private const string PREFIX_CHAR = "!";
#endif


    public TwitchChatService(ILogger<TwitchChatService> log, BrotatoItemEngine itemSearchEngine, IServiceProvider serviceProvider)
    {
        _credentials = new ConnectionCredentials("the_brotato_bot", "oauth:gx244vysokmuqcunclv8fovswigefo");
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
        _log.LogInformation($"Connected to {e.AutoJoinChannel}");
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
                    var currentRun = _serviceProvider.GetRequiredService<CurrentRun>();
                    var runData = currentRun.Current?.RunData;

                    if (runData is not null)
                    {
                        var charName = runData.Character.Replace("character_", "");
                        var niceCharName = string.Join(' ', charName.Split('_').Select(word => word.UcFirst()));

                        _client.SendMessage(e.ChatMessage.Channel, $"{niceCharName} - https://brotato.celerity.tv/current_run");
                    }
                    break;
                }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Initialize(_credentials, "celerity");

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
}
