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
    public interface IRolesService
    {
        Task<IEnumerable<Roles>> GetDataAsync();
    }
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RolesService(IRolesRepository rolesRepository, IUnitOfWork unitOfWork)
        {
            _rolesRepository = rolesRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Roles>> GetDataAsync() => await _rolesRepository.GetAll();
    }
}
