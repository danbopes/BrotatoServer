using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrotatoServer.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using BrotatoServer.Hubs;
using BrotatoServer.Models.JSON;
using Newtonsoft.Json;
using BrotatoServer.Data;

namespace BrotatoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunsController : ControllerBase
    {
        private readonly BrotatoServerContext _context;
        private readonly ILogger<RunsController> _log;

        public RunsController(ILogger<RunsController> log, BrotatoServerContext context)
        {
            _log = log;
            _context = context;
        }

        // GET: api/Runs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRun()
        {
            if (_context.Run == null)
            {
                return NotFound();
            }
            
            var runs = await _context.Run.ToListAsync();

            return runs.Select(run => {
                var runData = JsonConvert.DeserializeObject<RunInformation>(run.RunData)!;

                runData.UserId = null;
                runData.Mods = null;

                return new
                {
                    run.Id,
                    run.Date,
                    run.CurrentRotation,
                    RunData = runData
                };
            }).ToList();
        }

        // GET: api/Runs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Run>> GetRun(Guid id)
        {
            if (_context.Run == null)
            {
                return NotFound();
            }
            var run = await _context.Run.FindAsync(id);

            if (run == null)
            {
                return NotFound();
            }

            return run;
        }

        // POST: api/Runs
        [HttpPost]
        public async Task<IActionResult> PostRun([FromBody] RunInformation runInfo)
        {
            /*using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var rawJson = await reader.ReadToEndAsync();
            var jsonData = JObject.Parse(rawJson);*/

            //var created = jsonData.Value<long>("created");

            var run = new Run
            {
                Id = Guid.NewGuid(),
                Date = DateTimeOffset.FromUnixTimeSeconds(runInfo.Created),
                CurrentRotation = true,
                RunData = JsonConvert.SerializeObject(runInfo)
            };

            _context.Run.Add(run);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRun", new { id = run.Id }, run);
        }

        [HttpPost]
        [Route("current")]
        public async Task<IActionResult> PostCurrentRun([FromServices] CurrentRun run, [FromBody] RunInformation runInfo)
        {
            //using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            //var rawJson = await reader.ReadToEndAsync();

            _log.LogInformation("Update Run: {Run}", runInfo.RunData?.Character);

            await run.UpdateRun(runInfo);

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
            if (_context.Run == null)
            {
                return NotFound();
            }
            var run = await _context.Run.FindAsync(id);
            if (run == null)
            {
                return NotFound();
            }

            _context.Run.Remove(run);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RunExists(Guid id)
        {
            return (_context.Run?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
