using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AuthDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // REGISTER USER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
                return BadRequest("Username and password are required.");

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("User already exists.");

            // Default role if none given
            if (string.IsNullOrWhiteSpace(user.Role))
                user.Role = "user";

            user.PasswordHash = HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully", user.Id, user.Username, user.Role });
        }

        // LOGIN USER
        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.PasswordHash))
                return BadRequest("Username and password are required.");

            var hashedPassword = HashPassword(loginRequest.PasswordHash);

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == loginRequest.Username &&
                u.PasswordHash == hashedPassword);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful",
                token
            });
        }

        // GET ALL USERS (no passwords)
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Username, u.Profile, u.MobileNo, u.Role })
                .ToListAsync();

            return Ok(users);
        }

        // JWT TOKEN GENERATOR WITH ROLE CLAIM
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("profile", user.Profile ?? string.Empty),
                new Claim("role", user.Role), // Role added here
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // PASSWORD HASHING
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
