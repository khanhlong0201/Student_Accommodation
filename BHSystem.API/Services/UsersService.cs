using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace BHSystem.API.Services
{
    public interface IUsersService
    {
        Task<Users?> LoginAsync(UserModel request);
        Task<IEnumerable<Users>> GetDataAsync();
        Task<bool> UpdateUserAsync(RequestModel entity);
    }
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UsersService(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
        {
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Users?> LoginAsync(UserModel request)
        {
            //request.Password = EncryptHelper.Decrypt(request.Password + ""); // giải mã pass
            Users user = new Users();
            user.UserName = request.UserName;
            user.Password = EncryptHelper.Decrypt(request.Password + "");
            return await _usersRepository.LoginAsync(user);
        }

        public async Task<IEnumerable<Users>> GetDataAsync() => await _usersRepository.GetAll();

        public async Task<bool> UpdateUserAsync(RequestModel entity)
        {
            bool isUpdate = false;
            Users user = JsonConvert.DeserializeObject<Users>(entity.Json+"")!;
            
            switch (entity.Type)
            {
                case "Add":
                    user.Password = EncryptHelper.Encrypt(user.Password+"");
                    await _usersRepository.Add(user);
                    await _unitOfWork.CompleteAsync();
                    isUpdate = true;
                    break;
                case "Update":
                    await _usersRepository.Add(user);
                    await _unitOfWork.CompleteAsync();
                    isUpdate = true;
                    break;
                default: break;
            }
            return isUpdate;
        }
    }
}
