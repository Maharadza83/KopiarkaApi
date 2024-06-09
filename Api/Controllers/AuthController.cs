using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            // SprawdŸ, czy u¿ytkownik o tej nazwie ju¿ istnieje w bazie danych
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return Conflict("U¿ytkownik o tej nazwie ju¿ istnieje.");
            }

            // SprawdŸ, czy has³o ma co najmniej 5 znaków
            if (user.Password.Length < 5)
            {
                return BadRequest("Has³o musi mieæ co najmniej 5 znaków.");
            }

            // Haszuj has³o
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Dodaj u¿ytkownika do bazy danych
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == user.Username);
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
            {
                return Unauthorized();
            }

            var userDto = new UserDto
            {
                Username = dbUser.Username
                // Skopiuj inne w³aœciwoœci, które s¹ potrzebne
            };

            var token = GenerateJwtToken(userDto);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("self")]
        public async Task<IActionResult> GetSelf()
        {
            // Ensure the token is passed in the "Token" header
            var token = Request.Headers["Token"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Missing authorization token" });
            }

            // Use JwtSecurityTokenHandler to validate and parse the token
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken;

            try
            {
                jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            }
            catch
            {
                return Unauthorized(new { Message = "Invalid token" });
            }

            if (jwtToken == null)
            {
                return Unauthorized(new { Message = "Invalid token" });
            }

            // Extract the username (assuming it's stored in the "sub" or "name" claim)
            var username = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value
                           ?? jwtToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { Message = "Invalid token claims" });
            }

            // Return the username
            return Ok(new { Username = username });

        }
    }
}

