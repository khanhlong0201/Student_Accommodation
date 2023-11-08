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
    public class ImagesDetailsController : ControllerBase
    {
        private readonly ILogger<ImagesDetailsController> _logger;
        private readonly IImagesDetailsService _imagesdetailsService;
        private readonly IConfiguration _configuration;
        public ImagesDetailsController(ILogger<ImagesDetailsController> logger, IImagesDetailsService imagesdetailsService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _imagesdetailsService = imagesdetailsService;
        }

        /// <summary>
        /// lấy danh sách chi tiết hình ảnh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _imagesdetailsService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "imagesdetailsController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetImageDetailByImageIdAsync")]
        //[Authorize] // khi nào gọi trên web tháo truyền token
        public async Task<IActionResult> GetImageDetailByImageIdAsync(int imageId)
        {
            try
            {
                var data = await _imagesdetailsService.GetImageDetailByImageIdAsync(imageId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "imagesdetailsController", "GetImageDetailByImageIdAsync");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        //[Authorize] khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                var check = await _imagesdetailsService.DeleteById(id);
                if (!check)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Không xóa được"
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã xóa dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "imagesdetailsController", "DeleteBoardingHouse");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }
    }
}
