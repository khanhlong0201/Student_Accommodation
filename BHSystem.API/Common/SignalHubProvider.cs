using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.SignalR;

namespace BHSystem.API.Common
{
    public class SignalHubProvider : Hub
    {
        /// <summary>
        /// override lại phương thức connect 
        /// Tùy biến lưu lại User để gửi thong báo theo User
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var httpCtx = Context.GetHttpContext();
            if (httpCtx != null)
            {
                var connectionId = Context.ConnectionId;
                var userIdentifier = httpCtx.Request.Headers["UserName"].ToString();
                // Lưu thông tin về kết nối của người dùng
                await Groups.AddToGroupAsync(connectionId, userIdentifier);
            }
        }

        /// <summary>
        /// disconnect Hub theo User
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            var httpCtx = Context.GetHttpContext();
            if (httpCtx != null)
            {
                var connectionId = Context.ConnectionId;
                var userIdentifier = httpCtx.Request.Headers["UserName"].ToString();
                // Lưu thông tin về kết nối của người dùng
                await Groups.RemoveFromGroupAsync(connectionId, userIdentifier);
            }
        }
    }
}
