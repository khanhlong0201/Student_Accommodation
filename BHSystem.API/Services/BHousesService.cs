using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Models;

namespace BHSystem.API.Services
{
    public interface IBHousesService
    {
        Task<IEnumerable<BoardingHouseModel>> GetDataAsync();
        Task<ResponseModel> CreateBoardingHousesAsync(RequestModel entity);
        Task<ResponseModel> UpdateBoardingHousesAsync(RequestModel entity);
        Task<bool> DeleteMulti(RequestModel entity);
    }
    public class BHousesService
    {
        private readonly IBoardingHousesRepository _boardinghousesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BHousesService(IBoardingHousesRepository boardinghousesRepository, IUnitOfWork unitOfWork)
        {
            _boardinghousesRepository = boardinghousesRepository;
            _unitOfWork = unitOfWork;
        }
    }
}
