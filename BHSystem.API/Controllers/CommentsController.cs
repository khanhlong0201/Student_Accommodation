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
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly ICommentsService _commentsService;
        private readonly IConfiguration _configuration;
        public CommentsController(ILogger<CommentsController> logger, ICommentsService commentsService, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _commentsService = commentsService;
        }

        /// <summary>
        /// lấy danh sách bình luận
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _commentsService.GetDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "commentsController", "Get");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

        }
    }
}
