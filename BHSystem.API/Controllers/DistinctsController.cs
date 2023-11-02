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
    public class DistinctsController : ControllerBase
    {
        private readonly ILogger<DistinctsController> _logger;
        private readonly IDistinctsService _distinctsService;
        private readonly IConfiguration _configuration;
        public DistinctsController(ILogger<DistinctsController> logger, IDistinctsService distinctsService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _distinctsService = distinctsService;
        }

        /// <summary>
        /// lấy danh sách quận huyện
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _distinctsService.GetDataAsync();
               
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "distinctsController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        /// <summary>
        /// lấy danh sách quận huyện
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetId")]
        public async Task<IActionResult> GetId(int Id)
        {
            try
            {
                var data = await _distinctsService.GetIdAsync(Id);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "distinctsController", "GetId");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }

        /// <summary>
        /// lấy danh sách quận huyện theo thành phố
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllByCity")]
        public async Task<IActionResult> GetAllByCity(int city_id)
        {
            try
            {
                var data = await _distinctsService.GetAllByCityAsync(city_id);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "distinctsController", "GetAllByCity");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
