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
    public interface IWardsService
    {
        Task<IEnumerable<Wards>> GetDataAsync();
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
    }
}
