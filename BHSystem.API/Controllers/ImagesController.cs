using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Services;
using BHSytem.Models;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;

namespace BHSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IImagesService _imagesService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ImagesController(ILogger<ImagesController> logger, IImagesService imagesService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _imagesService = imagesService;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
        [Route("UploadImages")]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> files, string subFolder)
        {
            try
            {
                if(files == null || !files.Any())
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Không có dữ liệu file đính kèm"
                    });
                }    
                //
                var result = new List<ImagesDetailModel>();
                string fileName = string.Empty;
                string path = $"{this._webHostEnvironment.WebRootPath}\\{subFolder}";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                foreach (var file in files)
                {
                    fileName = file.FileName; // trên kia mã hóa
                    string fullPath = Path.Combine(path, fileName);
                    using (var image = Image.Load(file.OpenReadStream()))
                    {
                        image.Mutate(m => m.Resize(810, 540));
                        await image.SaveAsync(fullPath);
                    }
                    result.Add(new ImagesDetailModel() { File_Name = fileName, File_Path = fullPath });
                }    
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserController", "Update");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }    
    }
}
