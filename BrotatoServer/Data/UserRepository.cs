using System.Runtime.CompilerServices;
using BrotatoServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BrotatoServer.Data;

public class UserRepository : IUserRepository
{
    private readonly BrotatoServerContext _db;

    public UserRepository(BrotatoServerContext db)
    {
        _db = db;
    }

    public async IAsyncEnumerable<string> GetAllChatUsersAsync([EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (var user in _db.Users
                           .Where(user => user.TwitchUsername != null && user.JoinedChat)
                           .Select(user => user.TwitchUsername)
                           .AsAsyncEnumerable()
                           .WithCancellation(ct))
        {
            yield return user!;
        }
    }

    public async Task EnsureUserAsync(User user)
    {
        var dbUser = await _db.Users.FindAsync(user.SteamId);

        if (dbUser is null)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return;
        }

        if (user.TwitchUsername != dbUser.TwitchUsername || user.TwitchId != dbUser.TwitchId)
        {
            dbUser.TwitchUsername = user.TwitchUsername;
            dbUser.TwitchId = user.TwitchId;
            _db.Entry(dbUser).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return;
        }
    }

    public async Task<User?> GetUserAsync(ulong steamId)
    {
        return await _db.Users.FindAsync(steamId);
    }

    public async Task UpdateUserAsync(User user)
    {
        _db.Entry(user).State = EntityState.Modified;

        await _db.SaveChangesAsync();
    }
}