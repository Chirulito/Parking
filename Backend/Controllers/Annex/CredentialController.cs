using Microsoft.EntityFrameworkCore;
using Share.DataTransferModels.Auth;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Share.Models;
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

            // If the user is null or the password does not match, log the failed attempt
            if (user == null || !VerifyPassword(login.Password, user.Salt, user.Password))
            {
                return Unauthorized("Invalid User or Password");
            }

            var roles = user.UsersRoles.Select(ur => ur.IdRoleNavigation.RoleName).ToList();

            // Retornamos Id, Nombre (supongo que es user.Name o user.Username, cambia si es distinto)
            return Ok(new
            {
                id = user.IdUser,
                name = user.Name,
                roles = roles
            });
        }

        // REGISTER API =============================================================================
        [HttpPost("Register")]
        // Protect endpoint with authorization
        public IActionResult Register([FromBody] LoginPost newUser)
        {
            // Checks if the email is already taken
            if (_context.Users.Any(u => u.Email == newUser.Email))
                return BadRequest("Username is already taken");

            // Generates a salt and hashes the password
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(newUser.Password, salt);

            var user = new User
            {
                // Username = newUser.Username,
                Password = hashedPassword,
                Salt = salt
            };

            // Adds the new user to the database
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("User was successfully created");
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
}

