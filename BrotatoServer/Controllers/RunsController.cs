using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrotatoServer.Models;
using BrotatoServer.Hubs;
using BrotatoServer.Models.JSON;
using Newtonsoft.Json;
using BrotatoServer.Data;
using BrotatoServer.Services;
using BrotatoServer.Utilities;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRun()
        {
            var runs = await _runRepository.GetAllRunsAsync();
            return Ok(runs);
        }

        // GET: api/Runs/5
        [HttpGet("{id}")]
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
        public async Task<IActionResult> PostRun([FromBody] RunInformation runInfo, [FromServices] CurrentRun currentRun)
        {
            var run = await _runRepository.AddRunAsync(runInfo);

            await currentRun.UpdateRun(null);

            return CreatedAtAction("GetRun", new { id = run.Id }, run);
        }

        [HttpPost]
        [Route("current")]
        public async Task<IActionResult> PostCurrentRun([FromServices] CurrentRun run, [FromBody] RunInformation runInfo)
        {
            _log.LogInformation("Update Run: {Run}", runInfo.RunData?.Character);

            var updated = await run.UpdateRun(runInfo);

            if (!updated)
                return Conflict();

            return Ok();
        }

        [HttpDelete]
        [Route("current")]
        public async Task<IActionResult> PostCurrentRun([FromServices] CurrentRun run)
        {
            _log.LogInformation("Delete Current Run Request");

            await run.UpdateRun(null);

            return Ok();
        }

        // DELETE: api/Runs/5
        [HttpDelete("{id}")]
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
