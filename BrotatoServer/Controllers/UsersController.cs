using BrotatoServer.Data;
using BrotatoServer.Models;
using BrotatoServer.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers;
using BrotatoServer.Models.DB;
using Microsoft.AspNetCore.Authorization;

namespace BrotatoServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly BrotatoServerContext _db;

    public UsersController(BrotatoServerContext db)
    {
        _db = db;
    }

    [HttpPost]
    [Route("api_key_for_user/{steamId}")]
    public async Task<IActionResult> GetApiKeyForUser([FromRoute] ulong steamId)
    {
        var len = (int?)Request.Headers.ContentLength;

        if (len is null or <= 0 or > 2048)
            return BadRequest();

        var ticketBuffer = new byte[len.Value];

        await Request.Body.ReadExactlyAsync(ticketBuffer, 0, len.Value);

        var res = await SteamworksUtilities.AuthenticateSession(ticketBuffer, steamId);

        if (!res)
            return Unauthorized();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.SteamId == steamId);
        if (user == null)
        {
            user = new User
            {
                SteamId = steamId,
                ApiKey = Guid.NewGuid()
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        return Ok(user.ApiKey);
    }

    [HttpGet]
    [Route("test")]
    [Authorize(AuthenticationSchemes = "Twitch")]
    public IActionResult Test()
    {
        return Ok();
    }
}
