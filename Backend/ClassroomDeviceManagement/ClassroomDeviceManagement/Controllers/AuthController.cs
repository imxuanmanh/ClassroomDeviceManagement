using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string role;

            // Kiểm tra tài khoản (tạm thời hardcode)
            if (request.Username == "admin" && request.Password == "admin")
            {
                role = "Admin";
            }
            else if (request.Username == "user" && request.Password == "user")
            {
                role = "User";
            }
            else
            {
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });
            }


            // Tạo token
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Thông tin trong token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Trả token cho client
            return Ok(new
            {
                token = tokenString,
                role = role,
                expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!))
            });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
