using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using GameServerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GameServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMongoCollection<User> _users;
    private readonly string _jwtSecret;

    public AuthController(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _users = database.GetCollection<User>("Users");
        _jwtSecret = config["JwtSettings:Secret"]!;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var existingUser = await _users.Find(u => u.Username == request.Username).FirstOrDefaultAsync();
        if (existingUser != null) return BadRequest(new { message = "Username đã tồn tại!" });

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _users.InsertOneAsync(newUser);
        return Ok(new { message = "Đăng ký thành công!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var user = await _users.Find(u => u.Username == request.Username).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest(new { message = "Sai username hoặc password!" });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token = tokenHandler.WriteToken(token), username = user.Username });
    }
}