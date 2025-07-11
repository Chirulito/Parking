using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Share.Models;

namespace Backend.Controllers.Core.Parking
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public SpotController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Spot
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Spot>>> GetSpots()
        {
            return await _context.Spots.ToListAsync();
        }

        // GET: api/Spot/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Spot>> GetSpot(int id)
        {
            var spot = await _context.Spots.FindAsync(id);

            if (spot == null)
            {
                return NotFound();
            }

            return spot;
        }

        // PUT: api/Spot/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpot(int id, Spot spot)
        {
            if (id != spot.IdSpot)
            {
                return BadRequest();
            }

            _context.Entry(spot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpotExists(id))
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

        // POST: api/Spot
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Spot>> PostSpot(Spot spot)
        {
            _context.Spots.Add(spot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpot", new { id = spot.IdSpot }, spot);
        }

        // DELETE: api/Spot/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpot(int id)
        {
            var spot = await _context.Spots.FindAsync(id);
            if (spot == null)
            {
                return NotFound();
            }

            _context.Spots.Remove(spot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpotExists(int id)
        {
            return _context.Spots.Any(e => e.IdSpot == id);
        }
    }
}
