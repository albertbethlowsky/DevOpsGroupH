using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_minitwit.Data;
using mvc_minitwit.Models;

namespace mvc_minitwit.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimelineDatasController : ControllerBase
    {
        private readonly MvcDbContext _context;

        public TimelineDatasController(MvcDbContext context)
        {
            _context = context;
        }

        // GET: api/TimelineDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimelineData>>> GetTimelineData()
        {
            return await _context.TimelineData.ToListAsync();
        }

        // GET: api/TimelineDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimelineData>> GetTimelineData(int id)
        {
            var timelineData = await _context.TimelineData.FindAsync(id);

            if (timelineData == null)
            {
                return NotFound();
            }

            return timelineData;
        }

        // PUT: api/TimelineDatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimelineData(int id, TimelineData timelineData)
        {
            if (id != timelineData.message_id)
            {
                return BadRequest();
            }

            _context.Entry(timelineData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimelineDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TimelineDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TimelineData>> PostTimelineData(TimelineData timelineData)
        {
            _context.TimelineData.Add(timelineData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimelineData", new { id = timelineData.message_id }, timelineData);
        }

        // DELETE: api/TimelineDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimelineData(int id)
        {
            var timelineData = await _context.TimelineData.FindAsync(id);
            if (timelineData == null)
            {
                return NotFound();
            }

            _context.TimelineData.Remove(timelineData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimelineDataExists(int id)
        {
            return _context.TimelineData.Any(e => e.message_id == id);
        }
    }
}
