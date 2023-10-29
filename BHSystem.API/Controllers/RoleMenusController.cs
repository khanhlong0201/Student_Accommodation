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
    public class RoleMenusController : ControllerBase
    {
        private readonly ILogger<RoleMenusController> _logger;
        private readonly IRoleMenusService _rolemenusService;
        private readonly IConfiguration _configuration;
        public RoleMenusController(ILogger<RoleMenusController> logger, IRoleMenusService rolemenusService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _rolemenusService = rolemenusService;
        }

        /// <summary>
        /// lấy danh sách quyền menu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _rolemenusService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "rolemenusController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
