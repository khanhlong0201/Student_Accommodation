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
    public interface IRoleMenusService
    {
        Task<IEnumerable<RoleMenus>> GetDataAsync();
        Task<ResponseModel> AddOrDeleteRoleMenu(RequestModel entity);
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

        public async Task<ResponseModel> AddOrDeleteRoleMenu(RequestModel entity)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<RoleMenus> lstUserRoles = JsonConvert.DeserializeObject<List<RoleMenus>>(entity.Json + "")!;
                switch (entity.Type)
                {
                    case "Add":
                        lstUserRoles.ForEach(async (item) =>
                        {
                            item.User_Create = entity.UserId;
                            item.Date_Create = DateTime.Now;
                            await _rolemenusRepository.Add(item);
                        });
                        await _unitOfWork.CompleteAsync();
                        response.StatusCode = 0;
                        response.Message = "Success";
                        break;

                    case "Delete":
                        await _rolemenusRepository.DeleteMulti(lstUserRoles);
                        await _unitOfWork.CompleteAsync();
                        response.StatusCode = 0;
                        response.Message = "Success";
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = -1;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
