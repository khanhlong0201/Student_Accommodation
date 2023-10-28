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
    public interface IUserService
    {
        Task<Users?> LoginAsync(Users request);
        Task<IEnumerable<Users>> GetDataAsync();
        Task<bool> UpdateUserAsync(RequestModel entity);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Users?> LoginAsync(Users request)
        {
            //request.Password = EncryptHelper.Decrypt(request.Password + ""); // giải mã pass
            return await _userRepository.LoginAsync(request);
        }

        public async Task<IEnumerable<Users>> GetDataAsync() => await _userRepository.GetAll();

        public async Task<bool> UpdateUserAsync(RequestModel entity)
        {
            bool isUpdate = false;
            Users user = new Users();
            switch (entity.Type)
            {
                case "Add":
                    await _userRepository.Add(user);
                    await _unitOfWork.CompleteAsync();
                    isUpdate = true;
                    break;
                case "Update":
                    await _userRepository.Add(user);
                    await _unitOfWork.CompleteAsync();
                    isUpdate = true;
                    break;
                default: break;
            }
            return isUpdate;
        }
    }
}
