using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Share.Models;
using Backend.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    public class GuestEntryRequestDto
    {
        public string LicensePlate { get; set; }
        public string BuildingName { get; set; }
        public string Type { get; set; } 
        public bool? Accommodation { get; set; }
    }

    public class RegisteredEntryRequestDto
    {
        public string LicensePlate { get; set; }
        public string BuildingName { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ParkingController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost("entry/registered")]
        public async Task<IActionResult> RegisteredEntry([FromBody] RegisteredEntryRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LicensePlate) || string.IsNullOrWhiteSpace(request.BuildingName))
                return BadRequest(new { message = "Request Data Incomplete." });

            var licensePlate = request.LicensePlate.Trim().ToUpper();
            var buildingName = request.BuildingName.Trim();

            var building = await _context.Buildings.FirstOrDefaultAsync(b => b.Name == buildingName);
            if (building == null)
                return NotFound(new { message = "Building (Parking Lot) not found)." });

            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
            if (vehicle == null)
                return NotFound(new { message = "Unregistered Vehicle." });

            var type = vehicle.Type?.Trim().ToLower();
            if (type != "car" && type != "motorcycle")
                return BadRequest(new { message = "Invalid vehicle type." });

            var accommodation = vehicle.Accommodation ?? false;

            return await ProcessEntry(licensePlate, type, accommodation, building.IdBuilding, true);
        }

        [HttpPost("entry/guest")]
        public async Task<IActionResult> GuestEntry([FromBody] GuestEntryRequestDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.LicensePlate) ||
                string.IsNullOrWhiteSpace(request.BuildingName) ||
                string.IsNullOrWhiteSpace(request.Type))
            {
                return BadRequest(new { message = "Innformation is missing in the request as a guest" });
            }

            var licensePlate = request.LicensePlate.Trim().ToUpper();
            var buildingName = request.BuildingName.Trim();
            var type = request.Type.Trim().ToLower();

            if (type != "car" && type != "motorcycle")
                return BadRequest(new { message = "Invalid vehicle type." });

            var accommodation = request.Accommodation ?? false;

            var building = await _context.Buildings.FirstOrDefaultAsync(b => b.Name == buildingName);
            if (building == null)
                return NotFound(new { message = "Building (Parking Lot) not found)." });

            var isRegistered = await _context.Vehicles.AnyAsync(v => v.LicensePlate == licensePlate);

            if (isRegistered)
            {
                return Conflict(new { message = "Vehicle is already registered. Use the registered vehicle entry endpoint." });
            }

            var isAlreadyInside = await _context.Occupancies
                .AnyAsync(o => o.LicensePlate == licensePlate);

            if (isAlreadyInside)
            {
                await LogAction(licensePlate, "denied", "Vehicle is already parked in one parking Lot");
                return Conflict(new { message = "Vehicle is already inside the parking Lot (Double Entry Incongruency)" });
            }

            var previousAttempt = await _context.Logs
                .AnyAsync(l => l.LicensePlate == licensePlate && l.Action == "attempt");

            if (previousAttempt)
            {
                await LogAction(licensePlate, "denied", "Second attempt to access with an unregistered vehicle.");
                return Conflict(new { message = "Second attempt to access with an unregistered vehicle." });
            }

            return await ProcessEntry(licensePlate, type, accommodation, building.IdBuilding, false);
        }

        [HttpPost("egress")]
        public async Task<IActionResult> VehicleExit([FromBody] RegisteredEntryRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LicensePlate))
                return BadRequest(new { message = "License plate is required." });

            var licensePlate = request.LicensePlate.Trim().ToUpper();

            var occupancy = await _context.Occupancies
                .FirstOrDefaultAsync(o => o.LicensePlate == licensePlate);

            if (occupancy == null)
            {
                return NotFound(new { message = "Vehicle not currently parked or found." });
            }

            _context.Occupancies.Remove(occupancy);
            await _context.SaveChangesAsync();

            await LogAction(licensePlate, "egress", "Vehicle exited the parking lot.");
            return Ok(new { message = "Salida registrada correctamente." });
        }

        private async Task<IActionResult> ProcessEntry(string licensePlate, string type, bool accommodation, int buildingId, bool isRegistered)
        {
            var isVehicleInside = await _context.Occupancies
                .AnyAsync(o => o.LicensePlate == licensePlate);

            if (isVehicleInside)
            {
                await LogAction(licensePlate, "denied", "Vehicle is already parked in one parking Lot");
                return Conflict(new { message = "Vehicle is already inside the parking Lot (Double Entry Incongruency)" });
            }

            var spotTypes = accommodation
                ? new[] { "accommodation" }
                : type == "motorcycle"
                    ? new[] { "motorcycle" }
                    : new[] { "car" };

            var spot = await _context.Spots
                .Where(s => s.IdBuilding == buildingId &&
                            spotTypes.Contains(s.Type) &&
                            !_context.Occupancies.Any(o => o.IdSpot == s.IdSpot))
                .FirstOrDefaultAsync();

            if (spot == null)
            {
                await LogAction(licensePlate, "denied", "Parking Lot is already full for this type of Vehicle.");
                return Conflict(new { message = "Parking Lot is already full for this type of Vehicle." });
            }

            var occupancy = new Occupancy
            {
                IdSpot = spot.IdSpot,
                LicensePlate = licensePlate,
                Type = type,
                Accommodation = accommodation,
                Timestamp = DateTime.UtcNow
            };

            _context.Occupancies.Add(occupancy);
            await _context.SaveChangesAsync();

            var action = isRegistered ? "entry" : "attempt";
            var description = isRegistered ? "Entrada de vehículo registrado." : "Entrada temporal permitida.";

            await LogAction(licensePlate, action, description);
            return Ok(new { message = $"Acceso {(isRegistered ? "permitido" : "temporal")} al parqueo. Espacio asignado: {spot.Code}" });
        }

        private async Task LogAction(string licensePlate, string action, string description)
        {
            var log = new Log
            {
                LicensePlate = licensePlate,
                Action = action,
                Timestamp = DateTime.UtcNow,
                Description = description
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
