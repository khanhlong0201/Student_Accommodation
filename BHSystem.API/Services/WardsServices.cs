using BHSystem.API.Common;
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
    public interface IWardsService
    {
        Task<IEnumerable<Wards>> GetDataAsync();
        Task<IEnumerable<WardModel>> GetAllByDistinctAsync(int distinct_id);
    }
    public class WardsService : IWardsService
    {
        private readonly IWardsRepository _wardsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public WardsService(IWardsRepository wardsRepository, IUnitOfWork unitOfWork)
        {
            _wardsRepository = wardsRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Wards>> GetDataAsync() => await _wardsRepository.GetAll();

        public async Task<IEnumerable<WardModel>> GetAllByDistinctAsync(int distinct_id)
        {
            var result = await _wardsRepository.GetAllByDistinctAsync(distinct_id);
            return result;
        }
    }
}
