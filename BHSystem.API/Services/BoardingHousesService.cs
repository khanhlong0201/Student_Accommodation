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
    public interface IBoardingHousesService
    {
        Task<IEnumerable<BoardingHouses>> GetDataAsync();
    }
    public class BoardingHousesService : IBoardingHousesService
    {
        private readonly IBoardingHousesRepository _boardinghousesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BoardingHousesService(IBoardingHousesRepository boardinghousesRepository, IUnitOfWork unitOfWork)
        {
            _boardinghousesRepository = boardinghousesRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<BoardingHouses>> GetDataAsync() => await _boardinghousesRepository.GetAll();
    }
}
