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
        Task<ResponseModel> UpdateUserAsync(RequestModel entity);
        Task<bool> DeleteMulti(RequestModel entity);
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
            user.Password = request.Password;
            return await _usersRepository.LoginAsync(user);
        }

        public async Task<IEnumerable<Users>> GetDataAsync() => await _usersRepository.GetAll();

        public async Task<ResponseModel> UpdateUserAsync(RequestModel entity)
        {
            ResponseModel response = new ResponseModel();
            Users user = JsonConvert.DeserializeObject<Users>(entity.Json+"")!;
            switch (entity.Type)
            {
                case "Add":
                    if(await _usersRepository.CheckContainsAsync(m=>m.UserName == user.UserName))
                    {
                        response.StatusCode = -1;
                        response.Message = "Tên đăng nhập đã tồn tại";
                        break;
                    }    
                    user.Password = EncryptHelper.Encrypt(user.Password+"");
                    await _usersRepository.Add(user);
                    await _unitOfWork.CompleteAsync();
                    response.StatusCode = 0;
                    response.Message = "Success";
                    break;
                case "Update":
                    var userEntity = await _usersRepository.GetSingleByCondition(m => m.UserId == user.UserId);
                    if (userEntity == null)
                    {
                        response.StatusCode = -1;
                        response.Message = "Không tìm thấy dữ liệu";
                        break;
                    }
                    userEntity.Address = user.Address;
                    userEntity.FullName = user.FullName;
                    userEntity.Phone = user.Phone;
                    userEntity.Email = user.Email;
                    userEntity.Date_Update = DateTime.Now;
                    userEntity.User_Update = entity.UserId;
                    _usersRepository.Update(userEntity);
                    await _unitOfWork.CompleteAsync();
                    response.StatusCode = 0;
                    response.Message = "Success";
                    break;
                default: break;
            }
            return response;
        }

        /// <summary>
        /// xóa dữ liệu thực chất cập nhật cột IsDelete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMulti(RequestModel entity)
        {
            List<UserModel> lstUsers = JsonConvert.DeserializeObject<List<UserModel>>(entity.Json+"")!;
            foreach (UserModel user in lstUsers)
            {
                var userEntity = await _usersRepository.GetSingleByCondition(m => m.UserId == user.UserId);
                if(userEntity != null)
                {
                    userEntity.IsDeleted = true;
                    userEntity.Date_Update = DateTime.Now;
                    userEntity.User_Update = entity.UserId;
                    _usersRepository.Update(userEntity);
                }
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
