using System.Collections.Concurrent;
using BrotatoServer.Models.JSON;
using BrotatoServer.Services;
using BrotatoServer.Utilities;
using Microsoft.AspNetCore.SignalR;

namespace BrotatoServer.Hubs;

public class CurrentRunProvider
{
    private readonly IHubContext<RunsHub, IRunHub> _runHub;

    public ConcurrentDictionary<string, RunData?> Current { get; } = new();

    public CurrentRunProvider(IHubContext<RunsHub, IRunHub> runHub)
    {
        _runHub = runHub;
    }
    
    public async Task UpdateRunAsync(string twitchUsername, RunData? newRun)
    {
        RunData? update = null;
        if (newRun is not null)
        {
            update = newRun;
        }
        
        Current[twitchUsername] = update;

        await _runHub.Clients.Group(twitchUsername).RunUpdate(update);
    }
}

public interface IRunHub
{
    Task RunUpdate(RunData? newRun);
}

public class RunsHub : Hub<IRunHub>
{
    private readonly CurrentRunProvider _currentRunProvider;

    public RunsHub(CurrentRunProvider currentRunProvider)
    {
        _currentRunProvider = currentRunProvider;
    }

    public override async Task OnConnectedAsync()
    {
        var twitchUsername = Context.GetHttpContext()?.Request.Query["twitchUsername"];

        if (string.IsNullOrEmpty(twitchUsername))
            throw new InvalidOperationException("No twitchUsername was provided.");
        
        await Groups.AddToGroupAsync(Context.ConnectionId, twitchUsername!);
        
        _currentRunProvider.Current.TryGetValue(twitchUsername!, out var currentRun);
        
        await Clients.Caller.RunUpdate(currentRun);
        await base.OnConnectedAsync();
    }
}