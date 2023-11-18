using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;

namespace BHSystem.API.Services
{
    public interface IMessagesService
    {
        Task CreateMessage(string pType, string pMessage, string pJText, int pUserCreate);
        Task CreateMessageTicket(int pUserCreate, Bookings pBooking);
        Task<IEnumerable<MessageModel>> GetUnReadMessageByUserAsync(int pUserId, bool pIsAll);
        Task<bool> UpdateReadMessage(RequestModel entity);
        Task CreateMessageApprovalRoom(int pUserCreate, Rooms pRoomId, string pType = "Tạo mới");
        Task CreateMessageApprovalOrDenyRoom(int pUserCreate, Rooms pRoomId, string pType = "Phê duyệt");
    }
    public class MessagesService : IMessagesService
    {
        private readonly IMessagesRepository _messageRepo;
        private readonly IRoomsRepository _roomsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<SignalHubProvider> _hubContext;
        private readonly IUsersRepository _usersRepository;
        private readonly IUserMessagesRepository _userMessagesRepository;
        private readonly IBHousesRepository _boardinghousesRepository;

        public MessagesService(IMessagesRepository messageRepo, IUnitOfWork unitOfWork
            , IHubContext<SignalHubProvider> hubContext, IRoomsRepository roomsRepository
            , IUsersRepository usersRepository, IUserMessagesRepository userMessagesRepository, IBHousesRepository boardinghousesRepository)
        {
            _messageRepo = messageRepo;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _roomsRepository = roomsRepository;
            _usersRepository = usersRepository;
            _userMessagesRepository = userMessagesRepository;
            _boardinghousesRepository = boardinghousesRepository;
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
                await _unitOfWork.BeginTransactionAsync();
                Messages entity = new Messages();
                entity.Type = pType;
                entity.Message = pMessage;
                entity.JText = pJText;
                entity.User_Create = pUserCreate;
                await _messageRepo.Add(entity);

                // thông báo cho những ai
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();
                // lấy ra người nào
                await _hubContext.Clients.Group("C1").SendAsync("ReceiveMessage", entity);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        /// <summary>
        /// gửi thông báo đến user chịu trách nhiệm trên phòng đó và các user admin
        /// </summary>
        /// <param name="pUserCreate"></param>
        /// <param name="pBooking"></param>
        /// <returns></returns>
        public async Task CreateMessageTicket(int pUserCreate, Bookings pBooking)
        {
            try
            {
                var roomEntity = await _roomsRepository.GetSingleByCondition(m => m.Id == pBooking.Room_Id);
                if (roomEntity == null) return;
                List<int> lstUserId = new List<int>();
                lstUserId.Add(roomEntity.User_Create ?? 1); // gửi cho user tạo phòng lấy user vãn lai
                var lstUserAdmin = await _usersRepository.GetAll(m => m.IsDeleted == false && m.Type == "Admin");
                // gửi cho user Admin
                if (lstUserAdmin != null && lstUserAdmin.Any()) lstUserId.AddRange(lstUserAdmin.Select(m => m.UserId));
                
                await _unitOfWork.BeginTransactionAsync();
                Messages entity = new Messages();
                entity.Type = "Booking";
                entity.Message = $"[{pBooking.FullName} - {pBooking.Phone}] đã đặt phòng [{roomEntity.Name}].";
                entity.JText = JsonConvert.SerializeObject(pBooking);
                entity.User_Create = pUserCreate;
                entity.Date_Create = DateTime.Now;
                await _messageRepo.Add(entity);
                await _unitOfWork.CompleteAsync();

                // duyệt qua các user không trùng nhau
                for(int i = 0; i < lstUserId.Distinct().Count(); i++)
                {
                    UserMessages userMessage = new UserMessages();
                    userMessage.Message_Id = entity.Id;
                    userMessage.UserId = lstUserId[i];
                    userMessage.IsReaded = false;
                    userMessage.User_Create = pUserCreate;
                    userMessage.Date_Create = DateTime.Now;
                    await _userMessagesRepository.Add(userMessage);
                    await _unitOfWork.CompleteAsync();
                    await _hubContext.Clients.Group($"{lstUserId[i]}").SendAsync("ReceiveMessage", entity);
                }
                await _unitOfWork.CommitAsync();

            }
            catch(Exception)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        /// <summary>
        /// khi tạo phòng hoặc cập nhật phòng -> tạo thông báo gửi đến các quản trị viên
        /// </summary>
        /// <param name="pUserCreate"></param>
        /// <param name="pRoomId"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public async Task CreateMessageApprovalRoom(int pUserCreate, Rooms pRoomId, string pType = "Tạo mới")
        {
            try
            {
                // gửi message cho Admin phê duyệt chứng từ
                List<int> lstUserId = new List<int>();
                var lstUserAdmin = await _usersRepository.GetAll(m => m.IsDeleted == false && m.Type == "Admin");
                // gửi cho user Admin
                if (lstUserAdmin != null && lstUserAdmin.Any()) lstUserId.AddRange(lstUserAdmin.Select(m => m.UserId));
                // lấy ra Name của User
                string fullName = "Hệ thống";
                Users? user = await _usersRepository.GetSingleByCondition(m => m.UserId == pUserCreate);
                if(user != null) fullName = user.FullName + "";
                await _unitOfWork.BeginTransactionAsync();
                Messages entity = new Messages();
                entity.Type = "Phê duyệt/Từ chối phòng";
                entity.Message = $"[{fullName}] đã {pType} phòng [{pRoomId.Name}]. Vui lòng Phê duyệt/Từ chối phòng.";
                entity.JText = JsonConvert.SerializeObject(new
                {
                    pRoomId.Id,
                    pRoomId.Name,
                    pRoomId.Boarding_House_Id,
                    pRoomId.Status
                });
                entity.User_Create = pUserCreate;
                entity.Date_Create = DateTime.Now;
                await _messageRepo.Add(entity);
                await _unitOfWork.CompleteAsync();

                // duyệt qua các user không trùng nhau
                for (int i = 0; i < lstUserId.Distinct().Count(); i++)
                {
                    UserMessages userMessage = new UserMessages();
                    userMessage.Message_Id = entity.Id;
                    userMessage.UserId = lstUserId[i];
                    userMessage.IsReaded = false;
                    userMessage.User_Create = pUserCreate;
                    userMessage.Date_Create = DateTime.Now;
                    await _userMessagesRepository.Add(userMessage);
                    await _unitOfWork.CompleteAsync();
                    await _hubContext.Clients.Group($"{lstUserId[i]}").SendAsync("ReceiveMessage", entity);
                }
                await _unitOfWork.CommitAsync();

            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        /// <summary>
        /// Duyệt hoặc từ chối phòng này
        /// </summary>
        /// <param name="pUserCreate"></param>
        /// <param name="pRoomId"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public async Task CreateMessageApprovalOrDenyRoom(int pUserCreate, Rooms pRoomId, string pType = "Phê duyệt")
        {
            try
            {
                var bHouseByUser = await _boardinghousesRepository.GetSingleByCondition(m => m.Id == pRoomId.Boarding_House_Id);
                if(bHouseByUser != null)
                {
                    // gửi message cho phòng của người tạo phê duyệt chứng từ
                    List<int> lstUserId = new List<int>();
                    lstUserId.Add(bHouseByUser.User_Id); // gửi thông báo cho trọ của người này
                    // lấy ra Name của User
                    string fullName = "Quản trị";
                    Users? user = await _usersRepository.GetSingleByCondition(m => m.UserId == pUserCreate);
                    if (user != null) fullName = user.FullName + "";
                    await _unitOfWork.BeginTransactionAsync();
                    Messages entity = new Messages();
                    entity.Type = "Phê duyệt/Từ chối phòng";
                    entity.Message = $"Quản trị viên: [{fullName}] đã {pType} phòng [{pRoomId.Name}].";
                    entity.JText = JsonConvert.SerializeObject(new
                    {
                        pRoomId.Id,
                        pRoomId.Name,
                        pRoomId.Boarding_House_Id,
                        pRoomId.Status
                    });
                    entity.User_Create = pUserCreate;
                    entity.Date_Create = DateTime.Now;
                    await _messageRepo.Add(entity);
                    await _unitOfWork.CompleteAsync();

                    // duyệt qua các user không trùng nhau
                    for (int i = 0; i < lstUserId.Distinct().Count(); i++)
                    {
                        UserMessages userMessage = new UserMessages();
                        userMessage.Message_Id = entity.Id;
                        userMessage.UserId = lstUserId[i];
                        userMessage.IsReaded = false;
                        userMessage.User_Create = pUserCreate;
                        userMessage.Date_Create = DateTime.Now;
                        await _userMessagesRepository.Add(userMessage);
                        await _unitOfWork.CompleteAsync();
                        await _hubContext.Clients.Group($"{lstUserId[i]}").SendAsync("ReceiveMessage", entity);
                    }
                    await _unitOfWork.CommitAsync();
                }    
                

            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
            }
        }    

        public async Task<IEnumerable<MessageModel>> GetUnReadMessageByUserAsync(int pUserId, bool pIsAll) => await _messageRepo.GetUnReadMessageByUser(pUserId, pIsAll);

        /// <summary>
        /// cập nhật message là đã đọc của user
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateReadMessage(RequestModel entity)
        {
            MessageModel message = JsonConvert.DeserializeObject<MessageModel>(entity.Json + "")!;
            var messageEntity = await _userMessagesRepository.GetSingleByCondition(m => m.UserId == entity.UserId && m.Message_Id == message.Id);
            if (messageEntity != null)
            {
                messageEntity.IsReaded = true;
                messageEntity.Date_Update = DateTime.Now;
                messageEntity.User_Update = entity.UserId;
                _userMessagesRepository.Update(messageEntity);
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
