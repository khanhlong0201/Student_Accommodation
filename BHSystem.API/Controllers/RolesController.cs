using BHSystem.API.Services;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BHSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IRolesService _rolesService;
        private readonly IConfiguration _configuration;

        public RolesController(ILogger<RolesController> logger, IRolesService rolesService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _rolesService = rolesService;
        }

        /// <summary>
        /// lấy danh sách quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _rolesService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "rolesController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateRole(RequestModel user)
        {
            try
            {
                ResponseModel response = await _rolesService.UpdateDataAsync(user);
                if (response.StatusCode != 0) return BadRequest(response);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Success"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController", "Update");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteRoles(RequestModel user)
        {
            try
            {
                await _rolesService.DeleteMulti(user);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã xóa dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Delete");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });
            }
        }
    }
}