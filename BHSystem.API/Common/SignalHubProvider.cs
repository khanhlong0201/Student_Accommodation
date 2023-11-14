using BHSytem.Models.Entities;
using Microsoft.AspNetCore.SignalR;

namespace BHSystem.API.Common
{
    public class SignalHubProvider : Hub
    {
        public void BroadcastEmployee(Users user)
        {
            _ = Clients.All.SendAsync("ReceiveUsers", user); // gửi cho user
        }

        public void BroadcasrMessage(Messages message)
        {
            _ = Clients.All.SendAsync("ReceiveMessage", message); // gửi message gì
        }
    }
}
