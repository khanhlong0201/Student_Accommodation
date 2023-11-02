using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Newtonsoft.Json;

namespace BHSystem.API.Services
{
    public interface IUserRolesService
    {
        Task<IEnumerable<UserRoles>> GetDataAsync();
        Task<ResponseModel> AddOrDeleteUserRole(RequestModel entity);
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

        public async Task<ResponseModel> AddOrDeleteUserRole(RequestModel entity)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<UserRoles> lstUserRoles = JsonConvert.DeserializeObject<List<UserRoles>>(entity.Json + "")!;
                switch (entity.Type)
                {
                    case "Add":
                        lstUserRoles.ForEach(async (item) =>
                        {
                            item.User_Create = entity.UserId;
                            item.Date_Create = DateTime.Now;
                            await _userrolesRepository.Add(item);
                        });
                        await _unitOfWork.CompleteAsync();
                        response.StatusCode = 0;
                        response.Message = "Success";
                        break;

                    case "Delete":
                        await _userrolesRepository.DeleteMulti(lstUserRoles);
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