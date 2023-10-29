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
    public class MenusController : ControllerBase
    {
        private readonly ILogger<MenusController> _logger;
        private readonly IMenusService _menusService;
        private readonly IConfiguration _configuration;
        public MenusController(ILogger<MenusController> logger, IMenusService menusService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _menusService = menusService;
        }

        /// <summary>
        /// lấy danh sách menu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _menusService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "menuController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
