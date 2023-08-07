﻿using Microsoft.AspNetCore.Mvc;
using BrotatoServer.Models;
using BrotatoServer.Hubs;
using BrotatoServer.Models.JSON;
using BrotatoServer.Data;
using BrotatoServer.Models.DB;
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
        private readonly TwitchService _twitchService;
        private readonly CurrentRunProvider _currentRunProvider;

        public RunsController(
            ILogger<RunsController> log,
            IRunRepository runRepository,
            TwitchService twitchService,
            CurrentRunProvider currentRunProvider)
        {
            _log = log;
            _runRepository = runRepository;
            _twitchService = twitchService;
            _currentRunProvider = currentRunProvider;
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
        public async Task<IActionResult> PostRun([FromBody] RunInformation runInfo)
        {
            var run = await _runRepository.AddRunAsync(runInfo);

            var user = HttpContext.GetUser();

            if (user.TwitchUsername is not null)
            {
                await _currentRunProvider.UpdateRunAsync( user.TwitchUsername, null);
                await HandleEndOfRoundEventsAsync(run.Id, runInfo);
            }

            return CreatedAtAction("GetRun", new { id = run.Id }, run);
        }

        private async Task HandleEndOfRoundEventsAsync(Guid runId, RunInformation runInfo)
        {
            var user = HttpContext.GetUser();

            if (user.Settings is null)
                return;
            
            if (runInfo.RunData.Won && !string.IsNullOrEmpty(user.Settings.OnRunWonMessage))
            {
                var runMessage = user.Settings.OnRunWonMessage
                    .Replace("%character%", runInfo.RunData.Character.CharIdToNiceName());

                if (runMessage.Contains("%streak%"))
                {
                    var streak = await _runRepository.GetStreakAsync(user.TwitchUsername!);
                    runMessage = runMessage.Replace("%streak%", streak.ToString());
                }

                _twitchService.SendMessage(user.TwitchUsername!, runMessage);
            }
            else if (!runInfo.RunData.Won && !string.IsNullOrEmpty(user.Settings.OnRunLostMessage))
            {
                var runMessage = user.Settings.OnRunLostMessage
                    .Replace("%character%", runInfo.RunData.Character.CharIdToNiceName());
                
                _twitchService.SendMessage(user.TwitchUsername!, runMessage);
            }

            if (runInfo.RunData.Won && user.Settings.ClipOnRunWon ||
                !runInfo.RunData.Won && user.Settings.ClipOnRunLost)
            {
                _ = Task.Run(async () =>
                {
                    if (user.Settings.ClipDelaySeconds > 0)
                        await Task.Delay(TimeSpan.FromSeconds(user.Settings.ClipDelaySeconds));

                    await _twitchService.TryClipAsync(runId, user);
                });
            }
        }

        [HttpPost]
        [Route("current/notify_start")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> PostCurrentNotifyStart()
        {
            var user = HttpContext.GetUser();

            if (user.Settings?.OnRunStartedMessage is null)
                return Ok();

            if (user.TwitchUsername is null)
                return Conflict("Twitch Username has not been set up");

            _currentRunProvider.Current.TryGetValue(user.TwitchUsername, out var currentRun);
            if (currentRun is null)
                return Conflict("No run currently happening.");
            
            var runMessage = user.Settings.OnRunStartedMessage
                .Replace("%character%", currentRun.Character.CharIdToNiceName())
                .Replace("%link%", user.TwitchUsername.GetCurrentRunUrlForUser());

            if (runMessage.Contains("%streak%"))
            {
                var streak = await _runRepository.GetStreakAsync(user.TwitchUsername);
                runMessage = runMessage.Replace("%streak%", streak.ToString());
            }

            _twitchService.SendMessage(user.TwitchUsername, runMessage);

            return Ok();
        }

        [HttpPost]
        [Route("current")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> PostCurrentRun([FromBody] RunInformation runInfo)
        {
            var user = HttpContext.GetUser();

            if (user.TwitchUsername is null)
                return Conflict("Twitch username has not been setup");
            
            _log.LogInformation("Update Run: {Run}", runInfo.RunData?.Character);

            await _currentRunProvider.UpdateRunAsync(user.TwitchUsername, runInfo.RunData);

            return Ok();
        }

        [HttpDelete]
        [Route("current")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<IActionResult> PostCurrentRun()
        {
            var user = HttpContext.GetUser();

            if (user.TwitchUsername is null)
                return Conflict("Twitch username has not been setup");
            
            _log.LogInformation("Delete Current Run Request");

            await _currentRunProvider.UpdateRunAsync(user.TwitchUsername, null);

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
