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
    public interface IRolesService
    {
        Task<IEnumerable<Roles>> GetDataAsync();
        Task<ResponseModel> UpdateDataAsync(RequestModel entity);
        Task<bool> DeleteMulti(RequestModel entity);
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

        public async Task<ResponseModel> UpdateDataAsync(RequestModel entity)
        {
            ResponseModel response = new ResponseModel();
            Roles role = JsonConvert.DeserializeObject<Roles>(entity.Json + "")!;
            switch (entity.Type)
            {
                case "Add":
                    if (await _rolesRepository.CheckContainsAsync(m => m.Name == role.Name))
                    {
                        response.StatusCode = -1;
                        response.Message = "Tên quyền đã tồn tại.";
                        break;
                    }
                    role.Date_Create = DateTime.Now;
                    role.User_Create = entity.UserId;
                    await _rolesRepository.Add(role);
                    await _unitOfWork.CompleteAsync();
                    response.StatusCode = 0;
                    response.Message = "Success";
                    break;
                case "Update":
                    var roleEntity = await _rolesRepository.GetSingleByCondition(m => m.Id == role.Id);
                    if (roleEntity == null)
                    {
                        response.StatusCode = -1;
                        response.Message = "Không tìm thấy dữ liệu";
                        break;
                    }
                    roleEntity.Name = role.Name;
                    roleEntity.Date_Update = DateTime.Now;
                    roleEntity.User_Update = entity.UserId;
                    _rolesRepository.Update(role);
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
            List<RoleModel> lstRoles = JsonConvert.DeserializeObject<List<RoleModel>>(entity.Json + "")!;
            foreach (RoleModel role in lstRoles)
            {
                var roleEntity = await _rolesRepository.GetSingleByCondition(m => m.Id == role.Id);
                if (roleEntity != null)
                {
                    roleEntity.IsDeleted = true;
                    roleEntity.Date_Update = DateTime.Now;
                    roleEntity.User_Update = entity.UserId;
                    _rolesRepository.Update(roleEntity);
                }
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
