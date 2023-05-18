using FinalProject_API.Libs;
using FinalProject_API.Models;
using FinalProject_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalProject_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        FinalProject_SOAContext db;
        IConfiguration config;
        private readonly IAuthService _authService;

        public AuthController(FinalProject_SOAContext db, IConfiguration config, IAuthService authService)
        {
            this.db = db;
            this.config = config;
            _authService = authService;
        }

        /// <summary>
        /// Gửi request để lấy token.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Đăng nhập hệ thống</returns>
        /// <remarks>
        /// Ví dụ:
        ///
        ///     POST /api/auth/login
        ///     {
        ///        "email": "something",
        ///        "password": "passss"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Lấy token thành công</response>
        /// <response code="400">Sai email hoặc mật khẩu</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] User model)
        {
            //mã hóa password thành chuỗi MD5
            //var password_md5 = GenerateMD5(model.Password);
            //lấy người dùng trong db
            var user = db.Users.FirstOrDefault(x => x.Email == model.Email &&
            x.Password == model.Password);
            //nếu tồn tại thì xử lý sing token

            if(user == null)
            {
                return BadRequest(new { code = 400, message = "Request không hợp lệ do sai email hoặc mật khẩu" });
            }

            if (user.Confirmed == false)
            {
                return BadRequest(new { code = 400, message = "Tài khoản chưa được xác nhận, vui lòng vào email xác nhận tài khoản" });
            }

            if (user != null)
            {
                //lấy key trong file cấu hình
                var key = config["Jwt:Key"];
                //mã hóa ky
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                //ký vào key đã mã hóa
                var signingCredential = new SigningCredentials(signingKey,
                SecurityAlgorithms.HmacSha256);
                //tạo claims chứa thông tin người dùng (nếu cần)
                var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Role,user.Phanquyen),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim("UserId",user.UserId.ToString()),
                    new Claim("UserName",user.Hoten.ToString())
                };
                //tạo token với các thông số khớp với cấu hình trong startup để validate
                var token = new JwtSecurityToken
                    (
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signingCredential,
                    claims: claims
                    );
                //sinh ra chuỗi token với các thông số ở trên
                var return_token = new JwtSecurityTokenHandler().WriteToken(token);
                //trả về kết quả cho client username và chuỗi token
                return new JsonResult(new
                {
                    email = user.Email,
                    username = user.Hoten,
                    role = user.Phanquyen,
                    userid = user.UserId,
                    token = return_token,
                });
            }
            //trả về lỗi
            return BadRequest(new { code = 400, message = "Request không hợp lệ do sai email hoặc mật khẩu" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User model)
        {
            var existingUser = _authService.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { code = 400, message = "Email đã tồn tại trong hệ thống" });
            }

            var user = new User
            {
                Email = model.Email,
                Password = model.Password,
                Hoten = model.Hoten,
                Diachi = model.Diachi,
                Sdt = model.Sdt,
            };
            db.Users.Add(user);
            db.SaveChanges();

            var confirmationCode = _authService.GenerateEmailConfirmationTokenAsync(user.UserId);

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.UserId, code = confirmationCode }, protocol: HttpContext.Request.Scheme);
            
            Mailer.SendEmail(model.Email, "Xác nhận tài khoản", $"Vui lòng xác nhận tài khoản bằng cách nhấp vào <a href='{callbackUrl}'>đây</a>.");

            return Ok(new { code = 200, message = "Đăng ký thành công, vui lòng xác nhận tài khoản tại email của bạn." });

        }

        [HttpGet("confirm-email")]
        public IActionResult ConfirmEmail(int userId, string code)
        {
            var user = _authService.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { message = "Không tìm thấy người dùng" });
            }

            var result = _authService.ConfirmEmailAsync(userId, code);

            if (result)
            {
                return Ok(new { message = "Xác nhận email thành công" });
            }
            else
            {
                return BadRequest(new { message = "Xác nhận email thất bại" });
            }
        }


        /// <summary>
        /// Lấy chi tiết người dùng hiên tại.
        /// </summary>
        /// <returns>Lấy chi tiết người dùng hiên tại</returns>
        /// <response code="200">Trả về thông tin người dùng hiện tại</response>
        /// <response code="401">Không có quyền sử dụng</response>
        [Authorize(Roles = "user, manager")]
        [HttpGet("get-account")]
        public IActionResult GetAccount()
        {
            var userId = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value;
            var username = User.Claims.Where(x => x.Type == "UserName").FirstOrDefault()?.Value;
            return new JsonResult(new
            {
                message = "Xin chào người dùng sử dụng",
                manhanvien = userId,
                tennhanvien = username
            });
        }
    }
}
