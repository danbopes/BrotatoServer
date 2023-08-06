using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrotatoServer.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SetupController : ControllerBase
{
    [HttpGet]
    [Route("twitch")]
    public IActionResult Twitch()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/setup"
        }, "Twitch");
    }



    [HttpGet]
    [Route("steam")]
    public IActionResult Steam()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/setup"
        }, "Steam");
    }
}
