using BrotatoServer.Models.JSON;
using BrotatoServer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BrotatoServer.Data;

public class RunRepository : IRunRepository
{
    private readonly BrotatoServerContext _context;

    public RunRepository(BrotatoServerContext context)
    {
        _context = context;
    }

    public async IAsyncEnumerable<FullRun> GetAllRunsAsync(string twitchUsername)
    {
        var runs = _context.Run
            .Where(run => run.User!.TwitchUsername == twitchUsername)
            .AsAsyncEnumerable();

        await foreach (var run in runs)
        {
            var runInfo = JsonConvert.DeserializeObject<RunInformation>(run.RunInformation)!;

            yield return new FullRun
            {
                Id = run.Id,
                Date = run.Date,
                CurrentRotation = run.CurrentRotation,
                RunData = runInfo.RunData
            };
        }
    }

    public async Task<FullRun?> GetRunAsync(Guid id)
    {
        var run = await _context.Run.FindAsync(id);

        if (run is null)
            return null;

        var runInfo = JsonConvert.DeserializeObject<RunInformation>(run.RunInformation)!;

        return new FullRun
        {
            Id = run.Id,
            Date = run.Date,
            CurrentRotation = run.CurrentRotation,
            RunData = runInfo.RunData
        };
    }

    public async Task<Run> AddRunAsync(RunInformation runInfo)
    {
        var run = new Run
        {
            Id = Guid.NewGuid(),
            Date = DateTimeOffset.FromUnixTimeSeconds(runInfo.Created),
            CurrentRotation = true,
            RunInformation = JsonConvert.SerializeObject(runInfo)
        };

        _context.Run.Add(run);
        await _context.SaveChangesAsync();

        return run;
    }

    public async Task<bool> UpdateCurrentRunAsync(RunInformation runInfo)
    {
        // The implementation of this method is not provided in the original code.
        // You can implement the required logic to update the current run here.
        // It should return true if the update is successful, otherwise false.
        return false;
    }

    public async Task<bool> DeleteCurrentRunAsync()
    {
        // The implementation of this method is not provided in the original code.
        // You can implement the required logic to delete the current run here.
        // It should return true if the deletion is successful, otherwise false.
        return false;
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

    public async IAsyncEnumerable<FullRun> GetLatestRunsAsync(string twitchUsername, int amount = 3)
    {
        var runs = _context.Run
            .Where(run => run.User!.TwitchUsername == twitchUsername)
            .Take(amount)
            .AsAsyncEnumerable();
        
        await foreach (var run in runs)
        {
            var runInfo = JsonConvert.DeserializeObject<RunInformation>(run.RunInformation)!;

            yield return new FullRun
            {
                Id = run.Id,
                Date = run.Date,
                CurrentRotation = run.CurrentRotation,
                RunData = runInfo.RunData
            };
        }
    }
}
