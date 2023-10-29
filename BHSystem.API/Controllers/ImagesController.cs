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
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IImagesService _imagesService;
        private readonly IConfiguration _configuration;
        public ImagesController(ILogger<ImagesController> logger, IImagesService imagesService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _imagesService = imagesService;
        }

        /// <summary>
        /// lấy danh sách hình ảnh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _imagesService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "imagesController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
