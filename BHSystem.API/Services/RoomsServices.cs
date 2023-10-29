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
    public interface IRoomsService
    {
        Task<IEnumerable<Rooms>> GetDataAsync();
    }
    public class RoomsService : IRoomsService
    {
        private readonly IRoomsRepository _roomsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RoomsService(IRoomsRepository roomsRepository, IUnitOfWork unitOfWork)
        {
            _roomsRepository = roomsRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Rooms>> GetDataAsync() => await _roomsRepository.GetAll();
    }
}
