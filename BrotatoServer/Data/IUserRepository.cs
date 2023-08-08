using BrotatoServer.Models;
using BrotatoServer.Models.DB;

namespace BrotatoServer.Data;

public interface IUserRepository
{
    IAsyncEnumerable<string> GetAllChatUsersAsync(CancellationToken ct = default);
    Task EnsureUserAsync(User user);
    Task<User?> GetUserAsync(ulong steamId);
    Task<User> GetOrCreateUserAsync(ulong steamId);
    Task<User?> GetUserByTwitchUsername(string twitchUsername);
    Task UpdateUserAsync(User user);
    Task SaveSettingsAsync(UserSettings userSettings);
    Task UpdateCustomDataAsync(ulong userId, string customData);
}