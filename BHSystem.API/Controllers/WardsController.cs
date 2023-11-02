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
    public class WardsController : ControllerBase
    {
        private readonly ILogger<WardsController> _logger;
        private readonly IWardsService _wardsService;
        private readonly IConfiguration _configuration;
        public WardsController(ILogger<WardsController> logger, IWardsService wardsService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _wardsService = wardsService;
        }

        /// <summary>
        /// lấy danh sách xã phường
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _wardsService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "wardsController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        /// <summary>
        /// lấy danh sách xã phường theo quận huyện
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByDistinct")]
        public async Task<IActionResult> GetAllByDistinct(int distinct_id)
        {
            try
            {
                var data = await _wardsService.GetAllByDistinctAsync(distinct_id);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "distinctsController", "GetAllByDistinct");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
