using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomDeviceManagement.Controllers
{
    /// <summary>
    /// Controller xử lý các chức năng xác thực người dùng như đăng nhập, đăng ký, tạo JWT.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AuthController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        /// <summary>
        /// Đăng ký người dùng mới.
        /// </summary>
        /// <param name="user">Thông tin người dùng cần đăng ký.</param>
        /// <returns>
        /// Trả về thông tin người dùng đã đăng ký nếu thành công.
        /// Nếu người dùng đã tồn tại, trả về lỗi 400.
        /// Nếu xảy ra lỗi hệ thống, trả về lỗi 500.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user)
        {
            try
            {
                var result = await _userService.AddUserAsync(user);
                if (result == null)
                    return StatusCode(500, "Thêm user thất bại");

                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("tồn tại"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi hệ thống: " + ex.ToString()); // log đầy đủ stacktrace
                return StatusCode(500, "Đã xảy ra lỗi hệ thống");
            }
        }

        /// <summary>
        /// Đăng nhập và tạo JWT token cho người dùng.
        /// </summary>
        /// <param name="request">Thông tin đăng nhập gồm username và password.</param>
        /// <returns>
        /// Trả về token JWT, thời gian hết hạn và thông tin người dùng nếu đăng nhập thành công.
        /// Nếu sai tài khoản hoặc mật khẩu, trả về lỗi 401.
        /// Nếu request không hợp lệ, trả về lỗi 400.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username hoặc Password không hợp lệ");

            string role;

            // Kiểm tra tài khoản
            UserDto? user = await _userService.AuthenticateAsync(request.Username, request.Password);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });
            }

            if (user.RoleId == Role.Admin)
            {
                role = "Admin";
            }
            else
            {
                role = "User";
            }

            // Tạo token
            var token = CreateToken(request.Username, role);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                expires = token.ValidTo,
                user = user
            });
        }

        /// <summary>
        /// Kiểm tra xem Auth API đang hoạt động.
        /// </summary>
        /// <returns>Trả về chuỗi thông báo xác nhận API đang chạy.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Auth API is running");
        }


        /// <summary>
        /// Tạo JWT token cho người dùng xác thực thành công.
        /// </summary>
        /// <param name="username">Tên đăng nhập của người dùng.</param>
        /// <param name="role">Vai trò (role) của người dùng.</param>
        /// <returns>JWT token chứa các thông tin xác thực và quyền hạn.</returns>
        private JwtSecurityToken CreateToken(string username, string role)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var keyStr = jwtSettings["Key"];
            if (string.IsNullOrEmpty(keyStr))
                keyStr = "FallbackKey123456"; // tránh Swagger crash

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            double expireMinutes = 60;
            if (!double.TryParse(jwtSettings["ExpireMinutes"], out expireMinutes))
                expireMinutes = 60;

            return new JwtSecurityToken(
                issuer: jwtSettings["Issuer"] ?? "MyApp",
                audience: jwtSettings["Audience"] ?? "MyAppUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );
        }
    }
}
