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
    public interface IRoomsService
    {
        Task<IEnumerable<Rooms>> GetDataAsync();
        Task<IEnumerable<RoomModel>> GetDataByBHouseAsync(int iBHouseId);
        Task<ResponseModel> UpdateDataAsync(RequestModel entity);
        Task<bool> DeleteMulti(RequestModel entity);
        Task<IEnumerable<RoomModel>> GetAllByStatusAsync(string type);
        Task<bool> UpdateStatusMulti(RequestModel entity);
    }
    public class RoomsService : IRoomsService
    {
        private readonly IRoomsRepository _roomsRepository;
        private readonly IImagesRepository _imageRepository;
        private readonly IImagesDetailsRepository _imageDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBHousesRepository _boardinghousesRepository;
        private readonly IMessagesService _messagesService;
        public RoomsService(IRoomsRepository roomsRepository, IImagesRepository imageRepository
            , IImagesDetailsRepository imageDetailRepository, IUnitOfWork unitOfWork, IBHousesRepository bHousesRepository
            , IMessagesService messagesService)
        {
            _roomsRepository = roomsRepository;
            _unitOfWork = unitOfWork;
            _imageRepository = imageRepository;
            _imageDetailRepository = imageDetailRepository;
            _boardinghousesRepository = bHousesRepository;
            _messagesService = messagesService;
        }
        
        public async Task<IEnumerable<Rooms>> GetDataAsync() => await _roomsRepository.GetAll();

        public async Task<IEnumerable<RoomModel>> GetDataByBHouseAsync(int iBHouseId) => await _roomsRepository.GetDataByBHouseAsync(iBHouseId);

        public async Task<ResponseModel> UpdateDataAsync(RequestModel entity)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                RoomModel oItem = JsonConvert.DeserializeObject<RoomModel>(entity.Json + "")!;

                switch (entity.Type)
                {
                    case "Add":
                        await createRoomAsync(entity, oItem);
                        response.StatusCode = 0;
                        response.Message = "Success";
                        await UpdateQtyBHouse(oItem.BHouseId); // Cập nhật lại số lượng phòng
                        break;
                    case "Update":
                        var roomEntity = await _roomsRepository.GetSingleByCondition(m => m.Id == oItem.Id);
                        if (roomEntity == null)
                        {
                            response.StatusCode = -1;
                            response.Message = "Không tìm thấy dữ liệu";
                            break;
                        }
                        
                        await updateRoomAsync(entity, oItem, roomEntity);
                        response.StatusCode = 0;
                        response.Message = "Success";
                        //await UpdateQtyBHouse(oItem.BHouseId); // Cập nhật lại số lượng phòng
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = -1;
                response.Message = ex.Message;
                await _unitOfWork.RollbackAsync();
            }
            return response;
        }

        private async Task createRoomAsync(RequestModel entity, RoomModel oItem)
        {
            await _unitOfWork.BeginTransactionAsync();
            // Add Images
            Images images = new Images();
            images.Type = "Room";
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
            Rooms oRoom = new Rooms();
            oRoom.Name = oItem.Name + "";
            oRoom.Boarding_House_Id = oItem.BHouseId;
            oRoom.Status = "Chờ xử lý";
            oRoom.Address = oItem.Address + "";
            oRoom.Description = oItem.Description + "";
            oRoom.Length = oItem.Length;
            oRoom.Width = oItem.Width;
            oRoom.Price = oItem.Price;
            oRoom.Image_Id = images.Id;
            oRoom.Date_Create = DateTime.Now;
            oRoom.User_Create = entity.UserId;
            await _roomsRepository.Add(oRoom);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitAsync();
            await _messagesService.CreateMessageApprovalRoom(entity.UserId, oRoom); // gửi thông báo
        }

        private async Task updateRoomAsync(RequestModel entity, RoomModel oItem, Rooms roomEntity)
        {
            await _unitOfWork.BeginTransactionAsync();
            // Add Images
            Images images = new Images();
            images.Type = "Room";
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
            string status = roomEntity.Status;
            roomEntity.Image_Id = images.Id;
            roomEntity.Status = status != "Chờ xử lý" ? "Chờ xử lý" : roomEntity.Status; // cập nhật thì đợi duyệt lại
            roomEntity.Address = oItem.Address + "";
            roomEntity.Name = oItem.Name + "";
            roomEntity.Description = oItem.Description + "";
            roomEntity.Length = oItem.Length;
            roomEntity.Width = oItem.Width;
            roomEntity.Price = oItem.Price;
            roomEntity.Date_Update = DateTime.Now;
            roomEntity.User_Update = entity.UserId;
            _roomsRepository.Update(roomEntity);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitAsync();

            if(status != "Chờ xử lý") await _messagesService.CreateMessageApprovalRoom(entity.UserId, roomEntity, "Cập nhật"); // gửi thông báo
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
                var boardinghousesEntity = await _roomsRepository.GetSingleByCondition(m => m.Id == boardingHouse.Id);
                if (boardinghousesEntity != null)
                {
                    boardinghousesEntity.IsDeleted = true;
                    boardinghousesEntity.Date_Update = DateTime.Now;
                    boardinghousesEntity.User_Update = entity.UserId;
                    _roomsRepository.Update(boardinghousesEntity);
                    await _unitOfWork.CompleteAsync();
                    await UpdateQtyBHouse(boardinghousesEntity.Boarding_House_Id);
                }
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<RoomModel>> GetAllByStatusAsync(string type)
        {
            return await _roomsRepository.GetAllByStatusAsync(type);

        }

        /// <summary>
        /// cập nhật trạng thái dữ liệu thực chất cập nhật cột status
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateStatusMulti(RequestModel entity)
        {
            List<RoomModel> lstRoom= JsonConvert.DeserializeObject<List<RoomModel>>(entity.Json + "")!;
            foreach (RoomModel room in lstRoom)
            {
                var roomEntity = await _roomsRepository.GetSingleByCondition(m => m.Id == room.Id);
                if (roomEntity != null)
                {
                    roomEntity.Status = entity.Type;//từ chối hoặc duyệt
                    roomEntity.Date_Update = DateTime.Now;
                    roomEntity.User_Update = entity.UserId;
                    _roomsRepository.Update(roomEntity);
                    await _unitOfWork.CompleteAsync();
                    await _messagesService.CreateMessageApprovalOrDenyRoom(entity.UserId, roomEntity, entity.Type+"");
                }
            }
            return true;
        }

        /// <summary>
        /// cập nhật sl phòng
        /// </summary>
        /// <param name="pBHouseId"></param>
        /// <returns></returns>
        private async Task UpdateQtyBHouse(int pBHouseId)
        {
            try
            {
                var bhouseEntity = await _boardinghousesRepository.GetSingleByCondition(m => m.Id == pBHouseId);
                if (bhouseEntity != null)
                {
                    var data = await _roomsRepository.GetAll(m => m.Boarding_House_Id == pBHouseId && m.IsDeleted == false);
                    bhouseEntity.Qty = data?.Count() ?? 0;
                    _boardinghousesRepository.Update(bhouseEntity);
                    await _unitOfWork.CompleteAsync();
                }
            }
            catch(Exception)
            {

            }
            
        }   
    }
}
