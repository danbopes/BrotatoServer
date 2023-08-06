using BrotatoServer.Models;

namespace BrotatoServer.Data;

public interface IUserRepository
{
    IAsyncEnumerable<string> GetAllChatUsersAsync(CancellationToken ct = default);
    Task EnsureUserAsync(User user);
    Task<User?> GetUserAsync(ulong steamId);
    Task UpdateUserAsync(User user);
}