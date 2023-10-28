using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Services;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BHSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UserController(ILogger<UserController> logger, IUserService userService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// lấy danh sách user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _userService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        /// <summary>
        /// đăng nhập thông tin
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserModel loginRequest)
        {
            try
            {
                if (loginRequest == null) return BadRequest();
                //loginRequest.Password = EncryptHelper.Decrypt(loginRequest.Password+""); // giải mã pass
                var response = await _userService.LoginAsync(loginRequest);
                if(response == null) return BadRequest(new
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Thông tin đăng nhập không hợp lệ"
                });
                var claims = new[]
                {
                    new Claim("UserId", response.UserId + ""),
                    new Claim("FullName", response.FullName + ""),
                    new Claim("Phone", response.Phone + ""),
                    new Claim("Email", response.Email + ""),
                }; // thông tin mã hóa (payload)
                // JWT: json web token: Header - Payload - SIGNATURE (base64UrlEncode(header) + "." + base64UrlEncode(payload), your - 256 - bit - secret)
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:JwtSecurityKey").Value + "")); // key mã hóa
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // loại mã hóa (Header)
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration.GetSection("Jwt:JwtExpiryInDays").Value)); // hết hạn token
                var token = new JwtSecurityToken(
                    _configuration.GetSection("Jwt:JwtIssuer").Value,
                    _configuration.GetSection("Jwt:JwtAudience").Value,
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );
                return Ok(new
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Success",
                    response.UserId,
                    response.FullName, // để hiện thị lên người dùng khỏi phải parse từ clainm
                    Token = new JwtSecurityTokenHandler().WriteToken(token) // token user
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Login");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Status = StatusCodes.Status200OK,
                        ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("Update")]
        //[Authorize] khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> CreateUser(RequestModel user)
        {
            try
            {
                await _userService.UpdateUserAsync(user);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Thêm thông tin"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Update");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }

    }
}
