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
        private readonly ILogger<BHousesController> _logger;
        private readonly IBHousesService _boardinghousesService;
        public BHousesController(ILogger<BHousesController> logger, IBHousesService boardinghousesService)
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
        public async Task<IActionResult> Get(int pUserId, bool pIsAdmin)
        {
            try
            {
                var data = await _boardinghousesService.GetDataAsync(pUserId, pIsAdmin);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "_boardinghousesController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateBoardingHouse(RequestModel model)
        {
            try
            {
                ResponseModel response = await _boardinghousesService.UpdateDataAsync(model);
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
        public async Task<IActionResult> DeleteBoardingHouse(RequestModel bhHouse)
        {
            try
            {
                await _boardinghousesService.DeleteMulti(bhHouse);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã xóa dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BHousesController", "Delete");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });
            }
        }

        [HttpPost]
        [Route("CliGetDataBHouse")]
        public async Task<IActionResult> CliGetDataBHouse(BHouseSearchModel oSearch)
        {
            try
            {
                var result = await _boardinghousesService.CliGetDataBHouse(oSearch);
                if(result == null || result.ListData == null || !result.ListData.Any())
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Không tìm thấy dữ liệu. Vui lòng làm mới lại trang"
                    });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BHousesController", "Delete");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });
            }
        }

        [HttpGet]
        [Route("CliGetDataDetails")]
        public async Task<IActionResult> CliGetDataDetails([FromQuery] int pRoomId)
        {
            try
            {
                var result = await _boardinghousesService.CliGetDataBHouseDetail(pRoomId);
                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Không tìm thấy dữ liệu. Vui lòng làm mới lại trang"
                    });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BHousesController", "CliGetDataDetails");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });
            }
        }
    }
}
