using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using Microsoft.AspNetCore.SignalR;

namespace BHSystem.API.Services
{
    public interface IMessagesService
    {

    }
    public class MessagesService : IMessagesService
    {
        private readonly IMessagesRepository _messageRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<SignalHubProvider> _hubContext;

        public MessagesService(IMessagesRepository messageRepo, IUnitOfWork unitOfWork, IHubContext<SignalHubProvider> hubContext)
        {
            _messageRepo = messageRepo;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        /// <summary>
        /// tạo messages
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="pMessage"></param>
        /// <param name="pJText"></param>
        /// <param name="pUserCreate"></param>
        /// <returns></returns>
        public async Task CreateMessage(string pType, string pMessage, string pJText, int pUserCreate)
        {
            try
            {
                Messages entity = new Messages();
                entity.Type = pType;
                entity.Message = pMessage;
                entity.JText = pJText;
                entity.User_Create = pUserCreate;
                await _messageRepo.Add(entity);

                // thông báo cho những ai
                await _unitOfWork.CompleteAsync();
                await _hubContext.Clients.Group("C1").SendAsync("ReceiveMessage", entity);
            }
            catch (Exception){ }
        }

        private List<int> getUserMessageByType(string pType)
        {
            switch (pType)
            {
                case Constants.HUB_TYPE_BOOKING:

                    break;
                case Constants.HUB_TYPE_ROOM:

                    break;
            }
            return null;
        }
    }
}
