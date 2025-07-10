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
    public class OccupancyController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public OccupancyController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Occupancy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Occupancy>>> GetOccupancies()
        {
            return await _context.Occupancies.ToListAsync();
        }

        // GET: api/Occupancy/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Occupancy>> GetOccupancy(int id)
        {
            var occupancy = await _context.Occupancies.FindAsync(id);

            if (occupancy == null)
            {
                return NotFound();
            }

            return occupancy;
        }

        // PUT: api/Occupancy/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOccupancy(int id, Occupancy occupancy)
        {
            if (id != occupancy.IdOccupancy)
            {
                return BadRequest();
            }

            _context.Entry(occupancy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OccupancyExists(id))
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

        // POST: api/Occupancy
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Occupancy>> PostOccupancy(Occupancy occupancy)
        {
            _context.Occupancies.Add(occupancy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOccupancy", new { id = occupancy.IdOccupancy }, occupancy);
        }

        // DELETE: api/Occupancy/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOccupancy(int id)
        {
            var occupancy = await _context.Occupancies.FindAsync(id);
            if (occupancy == null)
            {
                return NotFound();
            }

            _context.Occupancies.Remove(occupancy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OccupancyExists(int id)
        {
            return _context.Occupancies.Any(e => e.IdOccupancy == id);
        }
    }
}
