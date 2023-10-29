using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
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
    }
}
