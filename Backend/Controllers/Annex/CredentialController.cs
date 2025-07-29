using Microsoft.EntityFrameworkCore;
using Share.DataTransferModels.Auth;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Backend.Context;

namespace Backend.Controllers.Annex
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialController : ControllerBase
    {
        // Reads configuration settings from appsettings.json
        private readonly IConfiguration _config;
        // Database context for accessing the database
        private readonly DatabaseContext _context;

        // Constructor that initializes the configuration and database context
        public CredentialController(IConfiguration config, DatabaseContext context)
        {
            _config = config;
            _context = context;
        }

        // APPLICATION PROGRAMING INTERFACE =========================================================

        // AUTH API =================================================================================
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginPost login)
        {
            // Queries the database for a user with the provided username

            var user = _context.Users
                .Include(u => u.UsersRoles)
                    .ThenInclude(ur => ur.IdRoleNavigation)
                .FirstOrDefault(u => u.Email == login.Email);

            // Gets the IP address of the client making the request
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";


            if (user == null || !VerifyPassword(login.Password, user.Salt, user.Password))
            {
                return Unauthorized("Invalid User or Password");
            }

            var roles = user.UsersRoles.Select(ur => ur.IdRoleNavigation.RoleName).ToList();

            bool mustChangePassword = VerifyPassword("Ulacit123", user.Salt, user.Password);

            return Ok(new
            {
                id = user.IdUser,
                name = user.Name,
                roles = roles,
                mustChangePassword = mustChangePassword
            });
        }

        // REGISTER API =============================================================================
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterPost newUser)
        {
            if (newUser == null ||
                string.IsNullOrWhiteSpace(newUser.Email) ||
                string.IsNullOrWhiteSpace(newUser.Name) ||
                string.IsNullOrWhiteSpace(newUser.Identification) ||
                string.IsNullOrWhiteSpace(newUser.Password) ||
                string.IsNullOrWhiteSpace(newUser.Dob))
            {
                return BadRequest("All fields are required.");
            }

            if (!DateOnly.TryParse(newUser.Dob, out DateOnly dob))
            {
                return BadRequest("Invalid date format. Use YYYY-MM-DD.");
            }

            if (_context.Users.Any(u => u.Email == newUser.Email))
                return BadRequest("Email is already taken.");

            if (_context.Users.Any(u => u.Identification == newUser.Identification))
                return BadRequest("Identification is already taken.");

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(newUser.Password, salt);

            var user = new Share.Models.User
            {
                Name = newUser.Name.Trim(),
                Email = newUser.Email.Trim().ToLower(),
                Identification = newUser.Identification.Trim(),
                Dob = dob,
                Password = hashedPassword,
                Salt = salt
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User was successfully created.");
        }

        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto model)
        {
            var user = _context.Users.FirstOrDefault(u => u.IdUser == model.IdUser);
            if (user == null)
                return NotFound("User not found");

            byte[] newSalt = GenerateSalt();
            byte[] newHash = HashPassword(model.NewPassword, newSalt);

            user.Salt = newSalt;
            user.Password = newHash;

            _context.SaveChanges();

            return Ok("Password updated successfully");
        }

        // METHODS ==================================================================================

        // Method transforms plain text password into a hashed password using PBKDF2 with SHA256
        private byte[] HashPassword(string password, byte[] salt)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            return deriveBytes.GetBytes(32);
        }

        // Method verifies if the provided plain text password matches the stored hashed password in the database
        private bool VerifyPassword(string plainPassword, byte[] salt, byte[] storedHash)
        {
            var hashAttempt = HashPassword(plainPassword, salt);
            return storedHash.SequenceEqual(hashAttempt);
        }

        // Method generates a random salt using a cryptographically secure random number generator
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }
    }

    public class ChangePasswordDto
    {
        public int IdUser { get; set; }
        public string NewPassword { get; set; }
    }
}

