using BrotatoServer.Data;
using BrotatoServer.Models;
using BrotatoServer.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers;
using BrotatoServer.Models.DB;
using BrotatoServer.Models.JSON;
using BrotatoServer.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace BrotatoServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ISteamworksService _steamworksService;

    public UsersController(IUserRepository userRepository, ISteamworksService steamworksService)
    {
        _userRepository = userRepository;
        _steamworksService = steamworksService;
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

        var res = await _steamworksService.AuthenticateSessionAsync(ticketBuffer, steamId);

        if (!res)
            return Unauthorized();

        var user = await _userRepository.GetOrCreateUserAsync(steamId);

        return Ok(user.ApiKey);
    }

    [HttpPost]
    [Route("custom_data")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public async Task<IActionResult> CustomData([FromBody] CustomData customData)
    {
        var user = HttpContext.GetUser();

        await _userRepository.UpdateCustomDataAsync(user.SteamId, JsonConvert.SerializeObject(customData));

        return Ok();
    }

    [HttpGet]
    [Route("test")]
    [Authorize(AuthenticationSchemes = "Twitch")]
    public IActionResult Test()
    {
        return Ok();
    }
}
