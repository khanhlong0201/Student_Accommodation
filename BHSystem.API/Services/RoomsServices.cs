﻿using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
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
        Task<IEnumerable<RoomModel>> GetAllByBHouseAsync(int bhouse_id);
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
        public async Task<IEnumerable<RoomModel>> GetAllByBHouseAsync(int bhouse_id)
        {
            var result = await _roomsRepository.GetAllByBHouseAsync(bhouse_id);
            return result;
        }
    }
}
