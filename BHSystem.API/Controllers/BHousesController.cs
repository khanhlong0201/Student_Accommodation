using BHSystem.API.Services;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BHSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BHousesController : ControllerBase
    {
        private readonly ILogger<BoardingHousesController> _logger;
        private readonly IBoardingHousesService _boardinghousesService;
        public BHousesController(ILogger<BoardingHousesController> logger, IBoardingHousesService boardinghousesService)
        {
            _logger = logger;
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
        public async Task<IActionResult> CreateBoardingHouse(RequestModel model)
        {
            try
            {
                ResponseModel response = await _boardinghousesService.CreateBoardingHousesAsync(model);
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
    }
}
