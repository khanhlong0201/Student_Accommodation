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
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BHSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ILogger<RoomsController> _logger;
        private readonly IRoomsService _roomsService;
        private readonly IConfiguration _configuration;
        public RoomsController(ILogger<RoomsController> logger, IRoomsService roomsService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _roomsService = roomsService;
        }

        /// <summary>
        /// lấy danh sách phòng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _roomsService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "roomController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        /// <summary>
        /// lấy danh sách phòng theo BHouse
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDataByBHouse")]
        public async Task<IActionResult> GetDataByBHouse(int pBHouseId)
        {
            try
            {
                var data = await _roomsService.GetDataByBHouseAsync(pBHouseId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "roomController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        /// <summary>
        /// cập nhật thông tin phòng trọ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateBoardingHouse(RequestModel model)
        {
            try
            {
                ResponseModel response = await _roomsService.UpdateDataAsync(model);
                if (response.StatusCode != 0) return BadRequest(response);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Thêm thông tin"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BoardingHouseController", "Create");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteRoom(RequestModel bhHouse)
        {
            try
            {
                await _roomsService.DeleteMulti(bhHouse);
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


        /// <summary>
        /// lấy danh sách phòng theo trạng thái
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByStatus")]
        public async Task<IActionResult> GetAllByStatus(string type)
        {
            try
            {
                var data = await _roomsService.GetAllByStatusAsync(type);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "GetAllByStatus");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }



        [HttpPost]
        [Route("UpdateStatus")]
        // [Authorize] //khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> UpdateStatusMulti(RequestModel room)
        {
            try
            {
                await _roomsService.UpdateStatusMulti(room);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "UpdateStatusMulti");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }

    }
}
