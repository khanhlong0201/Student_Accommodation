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
    public interface IUserRolesService
    {
        Task<IEnumerable<UserRoles>> GetDataAsync();
    }
    public class UserRolesService : IUserRolesService
    {
        private readonly IUserRolesRepository _userrolesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserRolesService(IUserRolesRepository userrolesRepository, IUnitOfWork unitOfWork)
        {
            _userrolesRepository = userrolesRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<UserRoles>> GetDataAsync() => await _userrolesRepository.GetAll();
    }
}
