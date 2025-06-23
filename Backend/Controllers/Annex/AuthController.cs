using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Share.DataTransferModels.Auth;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Share.EntityModels.Auth;
using System.Security.Claims;
using Backend.Context;
using System.Text;

namespace Backend.Controllers.Annex
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Reads configuration settings from appsettings.json
        private readonly IConfiguration _config;
        // Database context for accessing the database
        private readonly DatabaseContext _context;

        // Constructor that initializes the configuration and database context
        public AuthController(IConfiguration config, DatabaseContext context)
        {
            _config = config;
            _context = context;
        }

        // APPLICATION PROGRAMING INTERFACE =========================================================

        // AUTH API =================================================================================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginPost login)
        {
            // Queries the database for a user with the provided username
            var user = _context.Users
                .Include(u => u.UsersRoles)
                    .ThenInclude(ur => ur.IdRoleNavigation)
                        .ThenInclude(r => r.RolesResources)
                            .ThenInclude(rr => rr.IdResourceNavigation)
                .FirstOrDefault(u => u.Username == login.Username);

            // Gets the IP address of the client making the request
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // If the user is null or the password does not match, log the failed attempt
            if (user == null || !VerifyPassword(login.Password, user.Salt, user.Password))
            {
                // Logs the failed login attempt if the user exits to prevent null references
                if (user != null)
                {
                    var failedLog = new Log
                    {
                        IdUser = user.IdUser,
                        Action = $"Failed Attempt: IP Address {ip}",
                        Timestamp = DateTime.UtcNow
                    };
                    _context.Logs.Add(failedLog);
                    _context.SaveChanges();
                }
                return Unauthorized("Invalid User or Password");
            }

            // If the user is found and the password matches, log the successful attempt
            var successLog = new Log
            {
                IdUser = user.IdUser,
                Action = $"Succesful Attempt: IP Address {ip}",
                Timestamp = DateTime.UtcNow
            };
            _context.Logs.Add(successLog);
            _context.SaveChanges();

            // Generates a JWT token for the user
            var token = GenerateJwtToken(user);
            return Ok(new { token, roles = user.UsersRoles.Select(ur => ur.IdRoleNavigation.Name).ToList() }); // REVISAR ESTO
        }

        // REGISTER API =============================================================================
        [HttpPost("register")]
        // Protect endpoint with authorization
        [Authorize(Roles = "Administrator")]
        public IActionResult Register([FromBody] LoginPost newUser)
        {
            // Checks if the username is already taken
            if (_context.Users.Any(u => u.Username == newUser.Username))
                return BadRequest("Username is already taken");

            // Generates a salt and hashes the password
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(newUser.Password, salt);

            var user = new User
            {
                Username = newUser.Username,
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

        // Method generates a JWT token for the user with claims for user ID, username, roles, and resources
        private string GenerateJwtToken(User user)
        {
            // Claims used to store user information in the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            // Adds roles to a claims list
            foreach (var role in user.UsersRoles.Select(ur => ur.IdRoleNavigation.Name))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Adds resources to a claims list
            foreach (var resource in user.UsersRoles
                .SelectMany(ur => ur.IdRoleNavigation.RolesResources)
                .Select(rr => rr.IdResourceNavigation.Name)
                .Distinct())
            {
                claims.Add(new Claim("resource", resource));
            }

            // Reads the JWT key from the configuration
            var jwtKey = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key configuration value is missing.");

            // Retrieves the signing credentials using the JWT key and HMAC SHA256 algorithm
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"]));

            // Creates a new JWT token with the specified claims, expiration time, and signing credentials
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

