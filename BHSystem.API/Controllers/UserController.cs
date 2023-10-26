using BHSystem.API.Common;
using BHSystem.API.Services;
using BHSytem.Models;
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
        private readonly IUserService _userService;
        private ILogger<UserController> _logger { get; set; }
        private readonly IConfiguration _configuration;
        public UserController(IUserService userService, ILogger<UserController> logger, IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _configuration = configuration;
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
                if (response.StatusCode != 0) return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    Status = StatusCodes.Status400BadRequest,
                    response.Message
                });
                var claims = new[]
                {
                    new Claim("UserId", response.Data.UserId + ""),
                    new Claim("FullName", response.Data.FullName + ""),
                    new Claim("Phone", response.Data.Phone + ""),
                    new Claim("Email", response.Data.Email + ""),
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
                    response.Data.UserId,
                    response.Data.FullName, // để hiện thị lên người dùng khỏi phải parse từ clainm
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
        public async Task<IActionResult> Update([FromBody] RequestModel request)
        {
            try
            {
                ResponseModel response = new ResponseModel();
                response = await _userService.UpdateUserAsync(request);
                if (response == null || response.Status != 0) return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = response?.Message ?? "Vui lòng liên hệ IT để được hổ trợ."
                });
                return Ok(response);
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
