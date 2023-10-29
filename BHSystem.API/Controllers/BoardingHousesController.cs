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
    }
}
