using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Services;
using BHSytem.Models;
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
    public class BookingsController : ControllerBase
    {
        private readonly ILogger<BookingsController> _logger;
        private readonly IBookingsService _bookingsService;
        private readonly IConfiguration _configuration;
        public BookingsController(ILogger<BookingsController> logger, IBookingsService bookingsService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _bookingsService = bookingsService;
        }

        /// <summary>
        /// lấy danh sách đặt phòng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByPhone")]
        public async Task<IActionResult> GetAllByPhone(string phone)
        {
            try
            {
                var data = await _bookingsService.GetAllByPhoneAsync(phone);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "_bookingsController", "GetAllByPhone");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        /// <summary>
        /// lấy lịch sử đặt phòng theo sdt
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get(string type, int pUserId, bool pIsAdmin)
        {
            try
            {
                var data = await _bookingsService.GetDataAsync(type, pUserId, pIsAdmin);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "_bookingsController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> CreateTicket(RequestModel user)
        {
            try
            {
                ResponseModel response = await _bookingsService.UpdateUserAsync(user);
                if (response.StatusCode != 0) return BadRequest(response);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Thêm thông tin"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BookingsController", "CreateTicket");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }

        [HttpPost]
        [Route("UpdateStatus")]
       // [Authorize] //khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> UpdateStatusMulti(RequestModel booking)
        {
            try
            {
                await _bookingsService.UpdateStatusMulti(booking);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BoardingHouseController", "UpdateStatusMulti");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }
    }
}
