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
    public interface IDistinctsService
    {
        Task<IEnumerable<Distincts>> GetDataAsync();
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
        
        public async Task<IEnumerable<Distincts>> GetDataAsync() => await _distinctsRepository.GetAll();
    }
}
