using BrotatoServer.Hubs;
using BrotatoServer.SearchEngine;
using BrotatoServer.Utilities;
using System.Net;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Models;

namespace BrotatoServer.Services;

public class TwitchChatService : BackgroundService
{
    private readonly ILogger<TwitchChatService> _log;
    private readonly BrotatoItemEngine _itemSearchEngine;
    private readonly CurrentRun _currentRun;
    private readonly TwitchClient _client;
    private readonly ConnectionCredentials _credentials;

    public TwitchChatService(ILogger<TwitchChatService> log, BrotatoItemEngine itemSearchEngine, CurrentRun currentRun)
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
        _currentRun = currentRun;
    }

    private void Client_OnLog(object sender, OnLogArgs e)
    {
        Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        Console.WriteLine($"Connected to {e.AutoJoinChannel}");
    }

    private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
        //client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
    }

    private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        if (string.IsNullOrEmpty(e.ChatMessage.Message))
            return;
        
        var words = e.ChatMessage.Message.Split(' ');

        switch (words[0])
        {
            case "!item":
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
            case "!tater":
                {
                    var runData = _currentRun.Current?.RunData;

                    if (runData is not null)
                    {
                        var charName = runData.Character.Replace("character_", "");
                        var niceCharName = string.Join('_', charName.Split('_').Select(word => word.UcFirst()));

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
        _client.OnJoinedChannel += Client_OnJoinedChannel;
        _client.OnMessageReceived += Client_OnMessageReceived;
        _client.OnConnected += Client_OnConnected;

        _client.Connect();
        await _itemSearchEngine.InitializeAsync();
    }
}
