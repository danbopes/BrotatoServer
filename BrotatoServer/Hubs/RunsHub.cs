using BrotatoServer.Models.JSON;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BrotatoServer.Hubs;

public class CurrentRun
{
    private readonly IHubContext<RunsHub, IRunHub> _runHub;
    public RunInformation? Current { get; private set; }

    public CurrentRun(IHubContext<RunsHub, IRunHub> runHub)
    {
        _runHub = runHub;
    }
    
    public async Task UpdateRun(RunInformation? newRun)
    {
        Current = newRun;

        if (Current is not null)
        {
            Current.UserId = null;
            Current.Mods = null;
        }

        await _runHub.Clients.All.RunUpdate(Current);
    }
}

public interface IRunHub
{
    Task RunUpdate(RunInformation? newRun);
}

public class RunsHub : Hub<IRunHub>
{
    private readonly CurrentRun _currentRun;

    public RunsHub(CurrentRun currentRun)
    {
        _currentRun = currentRun;
    }

    public override async Task OnConnectedAsync()
    {
        if (_currentRun.Current != null)
        {
            await Clients.Caller.RunUpdate(_currentRun.Current);
        }

        await base.OnConnectedAsync();
    }
}