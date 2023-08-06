using Microsoft.AspNetCore.Mvc;
using BrotatoServer.Models;
using BrotatoServer.Hubs;
using BrotatoServer.Models.JSON;
using BrotatoServer.Data;
using BrotatoServer.Services;
using BrotatoServer.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace BrotatoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunsController : ControllerBase
    {
        private readonly ILogger<RunsController> _log;
        private readonly IRunRepository _runRepository;

        public RunsController(ILogger<RunsController> log, IRunRepository runRepository)
        {
            _log = log;
            _runRepository = runRepository;
        }

        // GET: api/Runs
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRun()
        {
            var runs = await _runRepository.GetAllRunsAsync();
            return Ok(runs);
        }*/

        // GET: api/Runs/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FullRun>> GetRun(Guid id)
        {
            var run = await _runRepository.GetRunAsync(id);
            if (run == null)
            {
                return NotFound();
            }

            return run;
        }

        // POST: api/Runs
        [HttpPost]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> PostRun([FromBody] RunInformation runInfo, [FromServices] CurrentRunProvider currentRunProvider)
        {
            var run = await _runRepository.AddRunAsync(runInfo);

            var user = HttpContext.GetUser();

            if (user.TwitchUsername is not null)
                await currentRunProvider.UpdateRun(user.TwitchUsername, null);

            return CreatedAtAction("GetRun", new { id = run.Id }, run);
        }

        [HttpPost]
        [Route("current/notify_start")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> PostCurrentNotifyStart([FromServices] CurrentRunProvider currentRunProvider, [FromServices] TwitchChatService twitchChatService)
        {
#if DEBUG
            return Ok();
#endif

            var user = HttpContext.GetUser();

            if (user.TwitchUsername is null)
                return Conflict("Twitch Username has not been set up");


            currentRunProvider.Current.TryGetValue(user.TwitchUsername, out var currentRun);
            if (currentRun is null)
                return Conflict("No run currently happening.");

            var runMessage = $"New %character% run started! Follow along here: %link%"
                .Replace("%character%", currentRun.Character.CharIdToNiceName())
                .Replace("%link%", user.TwitchUsername.GetCurrentRunUrlForUser());

            twitchChatService.SendMessage(user.TwitchUsername, runMessage);

            return Ok();
        }

        [HttpPost]
        [Route("current")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> PostCurrentRun([FromServices] CurrentRunProvider currentRunProvider, [FromBody] RunInformation runInfo)
        {
            var user = HttpContext.GetUser();

            if (user.TwitchUsername is null)
                return Conflict("Twitch username has not been setup");
            
            _log.LogInformation("Update Run: {Run}", runInfo.RunData?.Character);

            await currentRunProvider.UpdateRun(user.TwitchUsername, runInfo.RunData);

            return Ok();
        }

        [HttpDelete]
        [Route("current")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> PostCurrentRun([FromServices] CurrentRunProvider currentRunProvider)
        {
            var user = HttpContext.GetUser();

            if (user.TwitchUsername is null)
                return Conflict("Twitch username has not been setup");
            
            _log.LogInformation("Delete Current Run Request");

            await currentRunProvider.UpdateRun(user.TwitchUsername, null);

            return Ok();
        }

        // DELETE: api/Runs/5
        [HttpDelete("{id:guid}")]
        //[Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> DeleteRun(Guid id)
        {
            var deleted = await _runRepository.DeleteRunAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
