using System.Runtime.CompilerServices;
using BrotatoServer.Models;
using BrotatoServer.Models.DB;
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
                           .AsNoTracking()
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
            _db.Entry(user).State = EntityState.Detached;
            return;
        }

        if (user.TwitchUsername != dbUser.TwitchUsername || user.TwitchId != dbUser.TwitchId || user.TwitchAccessToken != dbUser.TwitchAccessToken)
        {
            dbUser.TwitchUsername = user.TwitchUsername;
            dbUser.TwitchId = user.TwitchId;
            dbUser.TwitchAccessToken = user.TwitchAccessToken;
            _db.Entry(dbUser).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            _db.Entry(dbUser).State = EntityState.Detached;
            return;
        }
        _db.Entry(dbUser).State = EntityState.Detached;
    }

    public async Task<User?> GetUserAsync(ulong steamId)
    {
        return await _db.Users
            .Include(user => user.Settings)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.SteamId == steamId);
    }

    public async Task<User> GetOrCreateUserAsync(ulong steamId)
    {
        var user = await GetUserAsync(steamId);
        if (user is not null)
            return user;
        
        user = new User
        {
            SteamId = steamId,
            ApiKey = Guid.NewGuid()
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        _db.Entry(user).State = EntityState.Detached;

        return user;
    }

    public async Task<User?> GetUserByTwitchUsername(string twitchUsername)
    {
        return await _db
            .Users
            .Include(user => user.Settings)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.TwitchUsername == twitchUsername);
    }

    public async Task UpdateUserAsync(User user)
    {
        _db.Entry(user).State = EntityState.Modified;

        await _db.SaveChangesAsync();

        _db.Entry(user).State = EntityState.Detached;
    }

    public async Task SaveSettingsAsync(UserSettings userSettings)
    {
        if (await _db.UserSettings.AsNoTracking().AnyAsync(settings => settings.UserId == userSettings.UserId))
        {
            _db.Entry(userSettings).State = EntityState.Modified;
        }
        else
        {
            _db.UserSettings.Add(userSettings);
        }
        
        await _db.SaveChangesAsync();

        _db.Entry(userSettings).State = EntityState.Detached;
    }

    public async Task UpdateCustomDataAsync(ulong userId, string customData)
    {
        await _db.Users
            .Where(user => user.SteamId == userId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(r => r.CustomData, customData));
    }
}