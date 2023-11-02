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
using BHSytem.Models.Models;

namespace BHSystem.API.Services
{
    public interface IDistinctsService
    {
        Task<IEnumerable<Distincts>> GetDataAsync();
        Task<DistinctModel> GetIdAsync(int Id);
        Task<IEnumerable<DistinctModel>> GetAllByCityAsync(int city_id);
    }
    public class DistinctsService : IDistinctsService
    {
        private readonly IDistinctsRepository _distinctsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DistinctsService(IDistinctsRepository distinctsRepository, IUnitOfWork unitOfWork)
        {
            _distinctsRepository = distinctsRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<DistinctModel> GetIdAsync(int Id)
        {
            var result = await _distinctsRepository.GetById(Id);
            return null;
        }
        public async Task<IEnumerable<Distincts>> GetDataAsync()
        {
            var result = await _distinctsRepository.GetAll();
            //var mappedResult = _mapper.Map<IEnumerable<DistinctModel>>(result);
            return result;
        }

        public async Task<IEnumerable<DistinctModel>> GetAllByCityAsync(int city_id)
        {
            var result = await _distinctsRepository.GetAllByCityAsync(city_id);
            return result;
        }
    }
}
