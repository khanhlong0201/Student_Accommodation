using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Newtonsoft.Json;
using System.Data;

namespace BHSystem.API.Services
{
    public interface IBHousesService
    {
        Task<IEnumerable<BoardingHouseModel>> GetDataAsync(int pUserId, bool pIsAdmin);
        Task<ResponseModel> UpdateDataAsync(RequestModel entity);
        Task<bool> DeleteMulti(RequestModel entity);
        Task<CliResponseModel<CliBoardingHouseModel>?> CliGetDataBHouse(BHouseSearchModel oSearch);
        Task<CliBoardingHouseModel?> CliGetDataBHouseDetail(int pRoomId);
    }
    public class BHousesService : IBHousesService
    {
        private readonly IBHousesRepository _boardinghousesRepository;
        private readonly IImagesRepository _imageRepository;
        private readonly IImagesDetailsRepository _imageDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BHousesService(IBHousesRepository boardinghousesRepository, IImagesRepository imageRepository
            , IImagesDetailsRepository imageDetailRepository, IUnitOfWork unitOfWork)
        {
            _boardinghousesRepository = boardinghousesRepository;
            _imageRepository = imageRepository;
            _imageDetailRepository = imageDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BoardingHouseModel>> GetDataAsync(int pUserId, bool pIsAdmin) => await _boardinghousesRepository.GetAllAsync(pUserId, pIsAdmin);
        public async Task<ResponseModel> UpdateDataAsync(RequestModel entity)
        {
            ResponseModel response = new ResponseModel();
            
            try
            {
                BHouseModel oItem = JsonConvert.DeserializeObject<BHouseModel>(entity.Json + "")!;
                
                switch (entity.Type)
                {
                    case "Add":
                        await _unitOfWork.BeginTransactionAsync();
                        await createBHouseAsync(entity, oItem);
                        response.StatusCode = 0;
                        response.Message = "Success";
                        await _unitOfWork.CommitAsync();
                        break;
                    case "Update":
                        var bhouseEntity = await _boardinghousesRepository.GetSingleByCondition(m => m.Id == oItem.Id);
                        if (bhouseEntity == null)
                        {
                            response.StatusCode = -1;
                            response.Message = "Không tìm thấy dữ liệu";
                            break;
                        }
                        await _unitOfWork.BeginTransactionAsync();
                        await updateBHouseAsync(entity, oItem, bhouseEntity);
                        response.StatusCode = 0;
                        response.Message = "Success";
                        await _unitOfWork.CommitAsync();
                        break;
                    default: break;
                }
            }
            catch(Exception ex)
            {
                response.StatusCode = -1;
                response.Message = ex.Message;
                await _unitOfWork.RollbackAsync();
            }
            return response;
        }  
        
        private async Task createBHouseAsync(RequestModel entity, BHouseModel oItem)
        {
            // Add Images
            Images images = new Images();
            images.Type = "BHouse";
            images.User_Create = entity.UserId;
            images.Date_Create = DateTime.Now;
            await _imageRepository.Add(images); // thêm hình ảnh
            await _unitOfWork.CompleteAsync();
            for (int i = 0; i < oItem.ListFile!.Count(); i++)
            {
                ImagesDetails oImgDetail = new ImagesDetails();
                oImgDetail.Image_Id = images.Id;
                oImgDetail.Id = (i + 1);
                oImgDetail.Date_Create = DateTime.Now;
                oImgDetail.User_Create = entity.UserId;
                oImgDetail.File_Path = oItem.ListFile![i].File_Name;
                await _imageDetailRepository.Add(oImgDetail);
            }
            BoardingHouses oBHouse = new BoardingHouses();
            oBHouse.Name = oItem.Name + "";
            oBHouse.User_Id = entity.UserId;
            oBHouse.Ward_Id = oItem.Ward_Id;
            oBHouse.Adddress = oItem.Adddress + "";
            oBHouse.Image_Id = images.Id;
            oBHouse.Date_Create = DateTime.Now;
            oBHouse.User_Create = entity.UserId;
            await _boardinghousesRepository.Add(oBHouse);
            await _unitOfWork.CompleteAsync();
        }

        private async Task updateBHouseAsync(RequestModel entity, BHouseModel oItem, BoardingHouses bhouseEntity)
        {
            // Add Images
            Images images = new Images();
            images.Type = "BHouse";
            images.User_Create = entity.UserId;
            images.Date_Create = DateTime.Now;
            await _imageRepository.Add(images); // thêm hình ảnh
            await _unitOfWork.CompleteAsync();
            for (int i = 0; i < oItem.ListFile!.Count(); i++)
            {
                ImagesDetails oImgDetail = new ImagesDetails();
                oImgDetail.Image_Id = images.Id;
                oImgDetail.Id = (i + 1);
                oImgDetail.Date_Create = DateTime.Now;
                oImgDetail.User_Create = entity.UserId;
                oImgDetail.File_Path = oItem.ListFile![i].File_Name;
                await _imageDetailRepository.Add(oImgDetail);
            }

            bhouseEntity.Image_Id = images.Id;
            bhouseEntity.Adddress = oItem.Adddress + "";
            bhouseEntity.Name = oItem.Name + "";
            bhouseEntity.Ward_Id = oItem.Ward_Id;
            bhouseEntity.Date_Update = DateTime.Now;
            bhouseEntity.User_Update = entity.UserId;
            _boardinghousesRepository.Update(bhouseEntity);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// xóa dữ liệu thực chất cập nhật cột IsDelete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMulti(RequestModel entity)
        {
            List<BoardingHouseModel> lstBoardingHouse = JsonConvert.DeserializeObject<List<BoardingHouseModel>>(entity.Json + "")!;
            foreach (BoardingHouseModel boardingHouse in lstBoardingHouse)
            {
                var boardinghousesEntity = await _boardinghousesRepository.GetSingleByCondition(m => m.Id == boardingHouse.Id);
                if (boardinghousesEntity != null)
                {
                    boardinghousesEntity.IsDeleted = true;
                    boardinghousesEntity.Date_Update = DateTime.Now;
                    boardinghousesEntity.User_Update = entity.UserId;
                    _boardinghousesRepository.Update(boardinghousesEntity);
                }
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<CliResponseModel<CliBoardingHouseModel>?> CliGetDataBHouse(BHouseSearchModel oSearch) => await _boardinghousesRepository.GetDataPagination(oSearch);

        public async Task<CliBoardingHouseModel?> CliGetDataBHouseDetail(int pRoomId) => await _boardinghousesRepository.GetDataBHDetail(pRoomId);
    }
}
