using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Backend.Context;
using Share.Models;

namespace Backend.Controllers.Core.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserControllerByIdentification : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UserControllerByIdentification(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/User/by-identification/{identification}
        [HttpGet("by-identification/{identification}")]
        public async Task<ActionResult<UserDto>> GetUserByIdentification(string identification)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Identification == identification);

            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Identification = user.Identification,
                Name = user.Name,
                Email = user.Email,
                Dob = user.Dob.ToString("yyyy-MM-dd")
            };

            return Ok(userDto);
        }

        // PUT: api/User/by-identification/{identification}
        [HttpPut("by-identification/{identification}")]
        public async Task<IActionResult> PutUserByIdentification(string identification, UserUpdateDto updatedUser)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Identification == identification);

            if (existingUser == null)
                return NotFound();

            if (updatedUser.Name is not null)
                existingUser.Name = updatedUser.Name;

            if (updatedUser.Email is not null)
                existingUser.Email = updatedUser.Email;

            if (updatedUser.Dob is not null)
            {
                if (!DateOnly.TryParse(updatedUser.Dob, out var parsedDob))
                    return BadRequest("Formato de fecha inválido. Usa 'yyyy-MM-dd'.");

                existingUser.Dob = parsedDob;
            }

            if (updatedUser.ChangePassword)
            {
                var salt = GenerateSalt();
                var hash = HashPassword("Ulacit123", salt);
                existingUser.Salt = salt;
                existingUser.Password = hash;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult> PostUser(UserCreateDto userDto)
        {
            if (!DateOnly.TryParse(userDto.Dob, out var parsedDob))
                return BadRequest("Formato de fecha inválido. Usa 'yyyy-MM-dd'.");

            // Validar que identification no exista
            if (await _context.Users.AnyAsync(u => u.Identification == userDto.Identification))
                return BadRequest("La identificación ya está registrada.");

            // Validar que email no exista
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
                return BadRequest("El correo electrónico ya está registrado.");

            // Buscar rol por nombre
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == userDto.RoleName);
            if (role == null)
                return BadRequest("El rol especificado no existe.");

            var salt = GenerateSalt();
            var hash = HashPassword("Ulacit123", salt);

            var user = new User
            {
                Identification = userDto.Identification,
                Name = userDto.Name,
                Email = userDto.Email,
                Dob = parsedDob,
                Salt = salt,
                Password = hash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userRole = new UsersRole
            {
                IdUser = user.IdUser,
                IdRole = role.IdRole
            };
            _context.UsersRoles.Add(userRole);

            await _context.SaveChangesAsync();

            return Ok("Usuario creado correctamente.");
        }

        // DELETE: api/User/by-identification/{identification}
        [HttpDelete("by-identification/{identification}")]
        public async Task<IActionResult> DeleteUserByIdentification(string identification)
        {
            var user = await _context.Users
                .Include(u => u.Vehicles)
                .Include(u => u.UsersRoles)
                .FirstOrDefaultAsync(u => u.Identification == identification);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (user.Vehicles.Any())
                return BadRequest("Este usuario tiene vehículos asociados. Elimínelos primero.");

            // Eliminar manualmente los roles asociados
            _context.UsersRoles.RemoveRange(user.UsersRoles);

            // Ahora se puede eliminar el usuario
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            return deriveBytes.GetBytes(32);
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }
    }

    // DTOs =============================

    public class UserDto
    {
        public string Identification { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Dob { get; set; } = null!;
    }

    public class UserCreateDto
    {
        public string Identification { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Dob { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }

    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Dob { get; set; }
        public bool ChangePassword { get; set; } = false;
    }
}
