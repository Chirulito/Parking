using Backend.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Backend.Controllers.Annex
{
    [Route("api/[controller]")]
    [ApiController]
    public class OccupancyCountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DatabaseContext _context;

        public OccupancyCountController(IConfiguration config, DatabaseContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetAvailableSpots(int Id)
        {
            var availableSpots = await _context.Spots
                .Where(s => s.IdBuilding == Id)
                .Where(s => !_context.Occupancies.Any(o => o.IdSpot == s.IdSpot))
                .CountAsync();

            return Ok(new { BuildingId = Id, AvailableSpots = availableSpots });
        }

        [HttpGet("name")]
        public async Task<IActionResult> GetAvailableSpotsByBuildingName(string Name)
        {
            var availableSpots = await _context.Spots
                .Where(s => s.IdBuildingNavigation.Name == Name)
                .Where(s => !_context.Occupancies.Any(o => o.IdSpot == s.IdSpot))
                .CountAsync();

            return Ok(new { BuildingName = Name, AvailableSpots = availableSpots });
        }
    }
}
