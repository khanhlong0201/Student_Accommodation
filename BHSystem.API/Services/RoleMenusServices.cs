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
    public interface IRoleMenusService
    {
        Task<IEnumerable<RoleMenus>> GetDataAsync();
    }
    public class RoleMenusService : IRoleMenusService
    {
        private readonly IRoleMenusRepository _rolemenusRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RoleMenusService(IRoleMenusRepository rolemenusRepository, IUnitOfWork unitOfWork)
        {
            _rolemenusRepository = rolemenusRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<RoleMenus>> GetDataAsync() => await _rolemenusRepository.GetAll();
    }
}
