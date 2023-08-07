using System.Collections.Concurrent;
using BrotatoServer.Data;
using BrotatoServer.Models.JSON;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

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
    Task CustomDataUpdate(CustomData? newCustomData);
}

public class RunsHub : Hub<IRunHub>
{
    private readonly CurrentRunProvider _currentRunProvider;
    private readonly IUserRepository _userRepo;

    public RunsHub(CurrentRunProvider currentRunProvider, IUserRepository userRepo)
    {
        _currentRunProvider = currentRunProvider;
        _userRepo = userRepo;
    }

    public override async Task OnConnectedAsync()
    {
        var twitchUsername = Context.GetHttpContext()?.Request.Query["twitchUsername"].FirstOrDefault();

        if (string.IsNullOrEmpty(twitchUsername))
            throw new InvalidOperationException("No twitchUsername was provided.");
        
        await Groups.AddToGroupAsync(Context.ConnectionId, twitchUsername);
        
        _currentRunProvider.Current.TryGetValue(twitchUsername, out var currentRun);
        
        var user = await _userRepo.GetUserByTwitchUsername(twitchUsername);
        if (user?.CustomData is not null)
            await Clients.Caller.CustomDataUpdate(JsonConvert.DeserializeObject<CustomData>(user.CustomData));
        
        
        await Clients.Caller.RunUpdate(currentRun);

        await base.OnConnectedAsync();
    }
}