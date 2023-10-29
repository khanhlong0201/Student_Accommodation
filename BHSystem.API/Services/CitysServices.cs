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
    public interface ICitysService
    {
        Task<IEnumerable<Citys>> GetDataAsync();
    }
    public class CitysService : ICitysService
    {
        private readonly ICitysRepository _citysRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CitysService(ICitysRepository citysRepository, IUnitOfWork unitOfWork)
        {
            _citysRepository = citysRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Citys>> GetDataAsync() => await _citysRepository.GetAll();
    }
}
