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
    public interface IMenusService
    {
        Task<IEnumerable<Menus>> GetDataAsync();
        Task<IEnumerable<Menus>> GetMenuByRoleAsync(int pRoleId);
    }
    public class MenusService : IMenusService
    {
        private readonly IMenusRepository _menusRepository;
        private readonly IUnitOfWork _unitOfWork;
        public MenusService(IMenusRepository menusRepository, IUnitOfWork unitOfWork)
        {
            _menusRepository = menusRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Menus>> GetDataAsync() => await _menusRepository.GetAll();

        public async Task<IEnumerable<Menus>> GetMenuByRoleAsync(int pRoleId) => await _menusRepository.GetMenuByRoleAsync(pRoleId);
    }
}
