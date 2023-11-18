using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Services;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BHSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersService _userService;
        private readonly IConfiguration _configuration;
        public UsersController(ILogger<UsersController> logger, IUsersService userService, IConfiguration configuration)
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
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        ex.Message
                    });
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
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Thông tin đăng nhập không hợp lệ"
                });
                var claims = new[]
                {
                    new Claim("UserId", response.UserId + ""),
                    new Claim("UserName", response.UserName + ""),
                    new Claim("FullName", response.FullName + ""),
                    new Claim("Phone", response.Phone + ""),
                    new Claim("IsAdmin", response.Type + ""),
                }; // thông tin mã hóa (payload)
                // JWT: json web token: Header - Payload - SIGNATURE (base64UrlEncode(header) + "." + base64UrlEncode(payload), your - 256 - bit - secret)
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:JwtSecurityKey").Value + "")); // key mã hóa
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // loại mã hóa (Header)
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration.GetSection("Jwt:JwtExpiryInDays").Value)); // hết hạn token
                //var expiry = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration.GetSection("Jwt:JwtExpiryInDays").Value)); // hết hạn token test 1 phút hết tonken
                var token = new JwtSecurityToken(
                    _configuration.GetSection("Jwt:JwtIssuer").Value,
                    _configuration.GetSection("Jwt:JwtAudience").Value,
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
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
                        StatusCode = StatusCodes.Status200OK,
                        ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("Update")]
        [Authorize] // khi nào gọi trên web tháo truyền token
        public async Task<IActionResult> CreateUser(RequestModel user)
        {
            try
            {
                ResponseModel response = await _userService.UpdateUserAsync(user);
                if(response.StatusCode != 0) return BadRequest(response);
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

        [HttpPost]
        [Route("Delete")]
        [Authorize] // khi nào gọi trên web tháo truyền token
        public async Task<IActionResult> DeleteUsers(RequestModel user)
        {
            try
            {
                await _userService.DeleteMulti(user);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã xóa dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Delete");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }

        [HttpGet]
        [Route("GetUserByRole")]
        [Authorize] // khi nào gọi trên web tháo truyền token
        public async Task<IActionResult> GetUserByRole(int pRoleId)
        {
            try
            {
                var lstUserExistRole = await _userService.GetUserByRoleAsync(pRoleId);
                var listUser = await _userService.GetDataAsync();
                
                if(listUser == null || !listUser.Any())
                {
                    return StatusCode(StatusCodes.Status204NoContent, new
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Không tìm thấy dữ liệu"
                    });
                }
                Dictionary<string, string> pResult = new Dictionary<string, string>();
                IEnumerable<UserModel>? lstUserNotExistRole = new List<UserModel>();
                string sJsonExist = "";
                if (lstUserExistRole == null || !lstUserExistRole.Any())
                {
                    // nếu nhóm này chưa có user
                    // lấy hết user
                    lstUserNotExistRole = listUser;
                    sJsonExist = "";
                }    
                else
                {
                    // lấy user nào chưa có trong nhóm
                    sJsonExist = JsonConvert.SerializeObject(lstUserExistRole);
                    lstUserNotExistRole = listUser.Where(m => lstUserExistRole.All(g => g.UserId != m.UserId));
                }
                pResult.Add("oUserExists", sJsonExist);
                pResult.Add("oUserNotExists", JsonConvert.SerializeObject(lstUserNotExistRole));
                return Ok(pResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "GetUserByRole");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("Register")]
        //[Authorize] // khi nào gọi trên web tháo truyền token
        public async Task<IActionResult> RegisterUserForClientAsync(RequestModel user)
        {
            try
            {
                ResponseModel response = await _userService.RegisterUserForClientAsync(user);
                if (response.StatusCode != 0) return BadRequest(response);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đăng ký thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Register");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }

        /// <summary>
        /// lấy danh sách user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserById")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int pUserId)
        {
            try
            {
                var data = await _userService.GetUserById(pUserId);
                if(data == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }    
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Get");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        ex.Message
                    });
            }
        }

    }
}
