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
    public class BoardingHousesController : ControllerBase
    {
        private readonly ILogger<BoardingHousesController> _logger;
        private readonly IBoardingHousesService _boardinghousesService;
        private readonly IConfiguration _configuration;
        public BoardingHousesController(ILogger<BoardingHousesController> logger, IBoardingHousesService boardinghousesService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _boardinghousesService = boardinghousesService;
        }

        /// <summary>
        /// lấy danh sách trọ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _boardinghousesService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "_boardinghousesController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("Create")]
        //[Authorize] khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> CreateBoardingHouse(RequestModel model)
        {
            try
            {
                ResponseModel response = await _boardinghousesService.CreateBoardingHousesAsync(model);
                if (response.StatusCode != 0) return BadRequest(response);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã thêm thông tin"
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
        [Route("Update")]
        //[Authorize] khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> UpdateBoardingHouse(RequestModel model)
        {
            try
            {
                ResponseModel response = await _boardinghousesService.UpdateBoardingHousesAsync(model);
                if (response.StatusCode != 0) return BadRequest(response);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Thêm thông tin"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BoardingHouseController", "Update");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }


        [HttpPost]
        [Route("Delete")]
        //[Authorize] khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> DeleteBoardingHouse(RequestModel boardingHouse)
        {
            try
            {
                await _boardinghousesService.DeleteMulti(boardingHouse);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã xóa dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BoardingHouseController", "DeleteBoardingHouse");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }
    }
}
