using AutoMapper;
using BrotatoServer.Models.JSON;
using BrotatoServer.Models;
using BrotatoServer.Models.DB;
using BrotatoServer.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BrotatoServer.Data;

public class RunRepository : IRunRepository
{
    private readonly BrotatoServerContext _context;
    private readonly IMapper _mapper;

    public RunRepository(BrotatoServerContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IAsyncEnumerable<FullRun> GetAllRunsAsync(string twitchUsername)
    {
        return _context.Run
            .Where(run => run.User!.TwitchUsername == twitchUsername)
            .AsAsyncEnumerable()
            .Select(run => _mapper.Map<FullRun>(run));
    }

    public async Task<FullRun?> GetRunAsync(Guid id)
    {
        var run = await _context.Run.FindAsync(id);

        return run is null ? null : _mapper.Map<FullRun>(run);
    }

    public async Task<Run> AddRunAsync(ulong userId, RunInformation runInfo, string? customData)
    {
        var run = new Run
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Won = runInfo.RunData.Won,
            Date = DateTimeOffset.FromUnixTimeSeconds(runInfo.Created),
            CurrentRotation = true,
            RunInformation = JsonConvert.SerializeObject(runInfo),
            CustomData = customData
        };

        _context.Run.Add(run);
        await _context.SaveChangesAsync();

        return run;
    }

    public async Task<bool> DeleteRunAsync(Guid id)
    {
        var run = await _context.Run.FindAsync(id);
        if (run == null)
        {
            return false;
        }

        _context.Run.Remove(run);
        await _context.SaveChangesAsync();

        return true;
    }

    public IAsyncEnumerable<FullRun> GetLatestRunsAsync(string twitchUsername, int amount = 3)
    {
        return _context.Run
            .Where(run => run.User!.TwitchUsername == twitchUsername)
            .OrderByDescending(run => run.Date)
            .Take(amount)
            .AsAsyncEnumerable()
            .Select(run => _mapper.Map<FullRun>(run));
    }

    public async Task<int> GetStreakAsync(string userTwitchUsername)
    {
        return await _context.Run
            .Where(run => run.User!.TwitchUsername == userTwitchUsername 
                          && run.Won &&
                          run.Date > _context.Run
                              .Where(run2 => run2.User!.TwitchUsername == userTwitchUsername && !run2.Won)
                              .Max(run2 => run2.Date))
            .CountAsync();
    }
}
