using BrotatoServer.Models.JSON;
using BrotatoServer.Models;
using BrotatoServer.Models.DB;

namespace BrotatoServer.Data;

public interface IRunRepository
{
    IAsyncEnumerable<FullRun> GetAllRunsAsync(string twitchUsername);
    Task<FullRun?> GetRunAsync(Guid id);
    Task<Run> AddRunAsync(ulong userId, RunInformation runInfo, string? customData);
    Task<bool> DeleteRunAsync(Guid id);
    IAsyncEnumerable<FullRun> GetLatestRunsAsync(string twitchUsername, int amount);
    Task<int> GetStreakAsync(string userTwitchUsername);
}
