using BrotatoServer.Models.JSON;
using BrotatoServer.Models;

namespace BrotatoServer.Data;

public interface IRunRepository
{
    IAsyncEnumerable<FullRun> GetAllRunsAsync(string twitchUsername);
    Task<FullRun?> GetRunAsync(Guid id);
    Task<Run> AddRunAsync(RunInformation runInfo);
    Task<bool> UpdateCurrentRunAsync(RunInformation runInfo);
    Task<bool> DeleteCurrentRunAsync();
    Task<bool> DeleteRunAsync(Guid id);
    IAsyncEnumerable<FullRun> GetLatestRunsAsync(string twitchUsername, int amount);
}
