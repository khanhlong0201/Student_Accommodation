using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Services;
using BHSytem.Models;
using BHSytem.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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

        [HttpGet]
        [Route("GetMenuByRole")]
        public async Task<IActionResult> GetMenuByRole(int pRoleId)
        {
            try
            {
                var lstMenuExistRole = await _menusService.GetMenuByRoleAsync(pRoleId);
                var listMenu = await _menusService.GetDataAsync();

                if (listMenu == null || !listMenu.Any())
                {
                    return StatusCode(StatusCodes.Status204NoContent, new
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Không tìm thấy dữ liệu"
                    });
                }
                Dictionary<string, string> pResult = new Dictionary<string, string>();
                IEnumerable<Menus>? lstMenuNotExistRole = new List<Menus>();
                string sJsonExist = "";
                if (lstMenuExistRole == null || !lstMenuExistRole.Any())
                {
                    // nếu nhóm này chưa có menu
                    // lấy hết menu
                    lstMenuNotExistRole = listMenu;
                    sJsonExist = "";
                }
                else
                {
                    // lấy menu nào chưa có trong nhóm
                    sJsonExist = JsonConvert.SerializeObject(lstMenuExistRole);
                    lstMenuNotExistRole = listMenu.Where(m => lstMenuExistRole.All(g => g.MenuId != m.MenuId));
                }
                pResult.Add("oMenuExists", sJsonExist);
                pResult.Add("oMenuNotExists", JsonConvert.SerializeObject(lstMenuNotExistRole));
                return Ok(pResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MenuController", "GetMenuByRole");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        ex.Message
                    });
            }
        }

        [HttpGet]
        [Route("GetMenuByUser")]
        public async Task<IActionResult> GetMenuByUser(int pUserId)
        {
            try
            {
                var data = await _menusService.GetMenuByUserAsync(pUserId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MenusController", "GetMenuByUser");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
    }
}
