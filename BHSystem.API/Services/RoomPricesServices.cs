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
    public interface IRoomPricesService
    {
        Task<IEnumerable<RoomPriceModel>> GetDataAsync();
        Task<ResponseModel> CreateRoomPricesAsync(RequestModel entity);
        Task<ResponseModel> UpdateRoomPricesAsync(RequestModel entity);
        Task<bool> DeleteMulti(RequestModel entity);
    }
    public class RoomPricesService : IRoomPricesService
    {
        private readonly IRoomPricesRepository _roompricesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RoomPricesService(IRoomPricesRepository roompricesRepository, IUnitOfWork unitOfWork)
        {
            _roompricesRepository = roompricesRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<RoomPriceModel>> GetDataAsync() => await _roompricesRepository.GetAllAsync();

        public async Task<ResponseModel> CreateRoomPricesAsync(RequestModel model)
        {
            ResponseModel response = new ResponseModel();
            RoomPrices roomPrices = JsonConvert.DeserializeObject<RoomPrices>(model.Json + "")!;

            var roomPriceMax = _roompricesRepository.GetMax(); // lấy giá trị lớn nhất trong bảng roomprice
            int roomPriceIdMax = (roomPriceMax.Result == null) ? 1 : roomPriceMax.Result.Id;

            roomPrices.Id = roomPriceIdMax + 1;
            await _roompricesRepository.Add(roomPrices);
            await _unitOfWork.CompleteAsync();
            response.StatusCode = 0;
            response.Message = "Success";
            return response;
        }

        public async Task<ResponseModel> UpdateRoomPricesAsync(RequestModel model)
        {
            ResponseModel response = new ResponseModel();
            RoomPrices roomPrices = JsonConvert.DeserializeObject<RoomPrices>(model.Json + "")!;
            var roomPricesEntity = await _roompricesRepository.GetSingleByCondition(m => m.Id == roomPrices.Id);
            if (roomPricesEntity == null)
            {
                response.StatusCode = -1;
                response.Message = "Không tìm thấy dữ liệu";
                return response;
            }

            roomPricesEntity.Id = roomPrices.Id;
            roomPricesEntity.Room_Id = roomPrices.Room_Id;
            roomPricesEntity.Price = roomPrices.Price;
            roomPricesEntity.Date_Update = DateTime.Now;
            //roomPricesEntity.User_Update = model.UserId;
            _roompricesRepository.Update(roomPricesEntity);
            await _unitOfWork.CompleteAsync();
            response.StatusCode = 0;
            response.Message = "Success";
            return response;
        }

        /// <summary>
        /// xóa dữ liệu thực chất cập nhật cột IsDelete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMulti(RequestModel entity)
        {
            List<RoomPriceModel> lstRoomPrice = JsonConvert.DeserializeObject<List<RoomPriceModel>>(entity.Json + "")!;
            foreach (RoomPriceModel roomPrice in lstRoomPrice)
            {
                var roomPricesEntity = await _roompricesRepository.GetSingleByCondition(m => m.Id == roomPrice.Id);
                if (roomPricesEntity != null)
                {
                    roomPricesEntity.IsDeleted = true;
                    roomPricesEntity.Date_Update = DateTime.Now;
                    roomPricesEntity.User_Update = entity.UserId;
                    _roompricesRepository.Update(roomPricesEntity);
                }
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
