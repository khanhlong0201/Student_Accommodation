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
    public class RoomPricesController : ControllerBase
    {
        private readonly ILogger<RoomPricesController> _logger;
        private readonly IRoomPricesService _roompricesService;
        private readonly IConfiguration _configuration;
        public RoomPricesController(ILogger<RoomPricesController> logger, IRoomPricesService roompricesService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _roompricesService = roompricesService;
        }

        /// <summary>
        /// lấy danh sách giá phòng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _roompricesService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "roompricesController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
