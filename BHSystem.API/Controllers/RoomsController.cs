using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
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
        /// lấy danh sách quận huyện theo thành phố
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByBHouse")]
        public async Task<IActionResult> GetAllByBHouse(int bhouse_id)
        {
            try
            {
                var data = await _roomsService.GetAllByBHouseAsync(bhouse_id);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "roomController", "GetAllByBHouse");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
