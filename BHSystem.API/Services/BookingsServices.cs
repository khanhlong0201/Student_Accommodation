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
        Task<IEnumerable<BookingModel>> GetDataAsync(string type, int pUserId, bool pIsAdmin);
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
        
        public async Task<IEnumerable<BookingModel>> GetDataAsync(string type, int pUserId, bool pIsAdmin)
        {
            return await _bookingsRepository.GetAllAsync(type, pUserId, pIsAdmin);
          
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
    }
}
