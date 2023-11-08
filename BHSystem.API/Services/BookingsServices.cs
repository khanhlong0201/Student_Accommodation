using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace BHSystem.API.Services
{
    public interface IBookingsService
    {
        Task<IEnumerable<Bookings>> GetDataAsync();
        Task<ResponseModel> UpdateUserAsync(RequestModel entity);
    }
    public class BookingsService : IBookingsService
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BookingsService(IBookingsRepository bookingsRepository, IUnitOfWork unitOfWork)
        {
            _bookingsRepository = bookingsRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Bookings>> GetDataAsync() => await _bookingsRepository.GetAll();

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
                    break;
                default: break;
            }
            return response;
        }
    }
}
