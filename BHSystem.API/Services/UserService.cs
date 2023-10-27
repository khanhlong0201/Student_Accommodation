using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models;
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
        Task<UserModel?> LoginAsync(UserModel request);
        Task<IEnumerable<UserModel>> GetDataAsync();
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
        public async Task<UserModel?> LoginAsync(UserModel request)
        {
            //request.Password = EncryptHelper.Decrypt(request.Password + ""); // giải mã pass
            return await _userRepository.LoginAsync(request);
        }

        public async Task<IEnumerable<UserModel>> GetDataAsync() => await _userRepository.GetAll();

        public async Task<bool> UpdateUserAsync(RequestModel entity)
        {
            bool isUpdate = false;
            UserModel user = new UserModel();
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
