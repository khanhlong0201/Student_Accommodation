using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Text.Json.Serialization;

namespace BHSystem.API.Services
{
    public interface IBookingsService
    {
        Task<IEnumerable<BookingModel>> GetDataAsync(string type);
        Task<ResponseModel> UpdateUserAsync(RequestModel entity);
        Task<bool> UpdateStatusMulti(RequestModel entity);
        Task<IEnumerable<BookingModel>> GetAllByPhoneAsync(string phone);
    }
    public class BookingsService : IBookingsService
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomsRepository _roomsRepository;
        private readonly IHubContext<SignalHubProvider> _hubContext;
        private readonly IUsersRepository _usersRepository;
        private readonly IMessagesRepository _messageRepo;
        private readonly IMessagesService _messagesService;
        public BookingsService(IBookingsRepository bookingsRepository, IUnitOfWork unitOfWork, IMessagesService messagesService,
            IHubContext<SignalHubProvider> hubContext, IRoomsRepository roomsRepository
            , IUsersRepository usersRepository, IMessagesRepository messageRepo)
        {
            _roomsRepository = roomsRepository;
            _usersRepository = usersRepository;
            _messageRepo = messageRepo;
            _bookingsRepository = bookingsRepository;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _messagesService = messagesService;
        }
        
        public async Task<IEnumerable<BookingModel>> GetDataAsync(string type)
        {
            return await _bookingsRepository.GetAllAsync(type);
          
        }
        public async Task<IEnumerable<BookingModel>> GetAllByPhoneAsync(string phone)
        {
            return await _bookingsRepository.GetAllByPhoneAsync(phone);

        }


        public async Task<ResponseModel> UpdateUserAsync(RequestModel entity)
        {
            ResponseModel response = new ResponseModel();
            Bookings ticket = JsonConvert.DeserializeObject<Bookings>(entity.Json + "")!;
            switch (entity.Type)
            {
                case "Add":
                    ticket.Date_Create = DateTime.Now;
                    await _bookingsRepository.Add(ticket);
                    await _unitOfWork.CompleteAsync();
                    response.StatusCode = 0;
                    response.Message = "Success";
                    await _messagesService.CreateMessageTicket(entity.UserId, ticket);
                    break;
                default: break;
            }
            return response;
        }


        /// <summary>
        /// cập nhật trạng thái dữ liệu thực chất cập nhật cột status
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateStatusMulti(RequestModel entity)
        {
            List<BookingModel> lstBooking = JsonConvert.DeserializeObject<List<BookingModel>>(entity.Json + "")!;
            foreach (BookingModel booking in lstBooking)
            {
                var bookingEntity = await _bookingsRepository.GetSingleByCondition(m => m.Id == booking.Id);
                if (bookingEntity != null)
                {
                    bookingEntity.Status = entity.Type;//từ chối hoặc duyệt
                    bookingEntity.Date_Update = DateTime.Now;
                    bookingEntity.User_Update = entity.UserId;
                    _bookingsRepository.Update(bookingEntity);
                }
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }

        /// <summary>
        /// gửi thông báo đến user chịu trách nhiệm trên phòng đó và các user admin
        /// </summary>
        /// <param name="pUserCreate"></param>
        /// <param name="pBooking"></param>
        /// <returns></returns>
        private async Task createMessageTicket(int pUserCreate, Bookings pBooking)
        {
            try
            {
                var roomEntity = await _roomsRepository.GetSingleByCondition(m => m.Id == pBooking.Room_Id);
                if (roomEntity == null) return;
                List<int> lstUserId = new List<int>();
                lstUserId.Add(roomEntity.User_Create ?? 0); // gửi cho user tạo phòng
                var lstUserAdmin = await _usersRepository.GetAll(m => m.IsDeleted == false && m.Type == "Admin");
                // gửi cho user Admin
                if (lstUserAdmin != null && lstUserAdmin.Any()) lstUserId.AddRange(lstUserAdmin.Select(m => m.UserId));

                await _unitOfWork.BeginTransactionAsync();
                Messages entity = new Messages();
                entity.Type = "Booking";
                entity.Message = $"Đã có người đặt phòng [{roomEntity.Name}].";
                entity.JText = JsonConvert.SerializeObject(pBooking);
                entity.User_Create = pUserCreate;
                entity.Date_Create = DateTime.Now;
                await _messageRepo.Add(entity);
                await _unitOfWork.CompleteAsync();

                for (int i = 0; i < lstUserId.Count(); i++)
                {
                    UserMessages userMessage = new UserMessages();
                    userMessage.Message_Id = entity.Id;
                    userMessage.UserId = lstUserId[i];
                    userMessage.IsReaded = false;
                    userMessage.User_Create = pUserCreate;
                    userMessage.Date_Create = DateTime.Now;
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
    }
}
