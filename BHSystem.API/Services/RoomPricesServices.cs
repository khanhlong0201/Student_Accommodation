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
    public interface IRoomPricesService
    {
        Task<IEnumerable<RoomPrices>> GetDataAsync();
    }
    public class RoomPricesService : IRoomPricesService
    {
        private readonly IRoomPricesRepository _roompricesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RoomPricesService(IRoomPricesRepository roompricesRepository, IUnitOfWork unitOfWork)
        {
            _roompricesRepository = roompricesRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<RoomPrices>> GetDataAsync() => await _roompricesRepository.GetAll();
    }
}
