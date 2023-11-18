using BHSystem.API.Services;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BHSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IMessagesService _messagesService;

        public MessagesController(ILogger<MessagesController> logger, IMessagesService messagesService)
        {
            _logger = logger;
            _messagesService = messagesService;
        }

        /// <summary>
        /// lấy danh sách tin nhắn chưa đọc theo user
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pIsAll">nếu IsAll == true => lấy tất cả theo user/ ngược lại lấy message chwua đọc</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUnReadMessageByUser")]
        public async Task<IActionResult> GetDataMessageByUser(int pUserId, bool pIsAll)
        {
            try
            {
                var data = await _messagesService.GetUnReadMessageByUserAsync(pUserId, pIsAll);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MessagesController", "GetUnReadMessageByUser");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("UpdateReadMessage")]
        // [Authorize] //khi nào gọi trên web tháo truỳen token
        public async Task<IActionResult> UpdateReadMessage(RequestModel booking)
        {
            try
            {
                await _messagesService.UpdateReadMessage(booking);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập dữ liệu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BoardingHouseController", "UpdateStatusMulti");
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ex.Message
                });

            }
        }
    }
}
