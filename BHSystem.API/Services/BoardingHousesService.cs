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
    public interface IBoardingHousesService
    {
        Task<IEnumerable<BoardingHouses>> GetDataAsync();
        Task<ResponseModel> CreateBoardingHousesAsync(RequestModel entity);
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
        public async Task<ResponseModel> CreateBoardingHousesAsync(RequestModel model)
        {
            ResponseModel response = new ResponseModel();
            BoardingHouses boardingHouses = JsonConvert.DeserializeObject<BoardingHouses>(model.Json + "")!;
            if (await _boardinghousesRepository.CheckContainsAsync(m => m.Name == boardingHouses.Name))
            {
                response.StatusCode = -1;
                response.Message = "Tên đăng nhập đã tồn tại";
                return response;
            }
            await _boardinghousesRepository.Add(boardingHouses);
            await _unitOfWork.CompleteAsync();
            response.StatusCode = 0;
            response.Message = "Success";
            return response;
        }
    }
}
