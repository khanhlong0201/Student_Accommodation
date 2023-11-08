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
    public interface IBoardingHousesService
    {
        Task<IEnumerable<BoardingHouseModel>> GetDataAsync();
        Task<ResponseModel> CreateBoardingHousesAsync(RequestModel entity);
        Task<ResponseModel> UpdateBoardingHousesAsync(RequestModel entity);
        Task<bool> DeleteMulti(RequestModel entity);
    }
    public class BoardingHousesService : IBoardingHousesService
    {
        private readonly IBoardingHousesRepository _boardinghousesRepository;
        private readonly IImagesRepository _imageRepository;
        private readonly IImagesDetailsRepository _imageDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BoardingHousesService(IBoardingHousesRepository boardinghousesRepository, IImagesRepository imageRepository, IImagesDetailsRepository imageDetailRepository, IUnitOfWork unitOfWork)
        {
            _boardinghousesRepository = boardinghousesRepository;
            _imageRepository = imageRepository;
            _imageDetailRepository = imageDetailRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<BoardingHouseModel>> GetDataAsync() => await _boardinghousesRepository.GetAllAsync();
        public async Task<ResponseModel> CreateBoardingHousesAsync(RequestModel model)
        {
            ResponseModel response = new ResponseModel();
            Images images = new Images();
            images.Type = "BHouse";
            images.User_Create = model.UserId;
            images.Date_Create = DateTime.Now;
            BoardingHouses boardingHouses = JsonConvert.DeserializeObject<BoardingHouses>(model.Json + "")!;
            if (await _boardinghousesRepository.CheckContainsAsync(m => m.Name == boardingHouses.Name))
            {
                response.StatusCode = -1;
                response.Message = "Tên trọ đã tồn tại";
                return response;
            }

            await _imageRepository.Add(images); // thêm hình ảnh
            await _unitOfWork.CompleteAsync();
            var imageMax = _imageRepository.GetMax(); // lấy giá trị lớn nhất trong bảng hình ảnh
            int imageIdMax = (imageMax == null && imageMax.Result == null)? 1: imageMax.Result.Id;

            var imageDetailMax = _imageDetailRepository.GetMax(); // lấy giá trị lớn nhất trong bảng hình ảnh
            int imageDetailIdMax = (imageDetailMax == null && imageDetailMax.Result ==null) ? 1:imageDetailMax.Result.Id;

            List<ImagesDetails> listImageDetail = JsonConvert.DeserializeObject<List<ImagesDetails>>(model.Json_Detail + "")!;
            listImageDetail.ForEach(async (item) =>
            {
                item.Date_Create = DateTime.Now;
                item.Image_Id = imageIdMax;
                item.Id = ++imageDetailIdMax;
                await _imageDetailRepository.Add(item);
            });


            
            boardingHouses.Image_Id = imageIdMax;
            await _boardinghousesRepository.Add(boardingHouses);
            await _unitOfWork.CompleteAsync();
            response.StatusCode = 0;
            response.Message = "Success";
            return response;
        }




        public async Task<ResponseModel> UpdateBoardingHousesAsync(RequestModel model)
        {
            ResponseModel response = new ResponseModel();
            BoardingHouses boardingHouses = JsonConvert.DeserializeObject<BoardingHouses>(model.Json + "")!;
            var boardingHousesEntity = await _boardinghousesRepository.GetSingleByCondition(m => m.Id == boardingHouses.Id);
            if (boardingHousesEntity == null)
            {
                response.StatusCode = -1;
                response.Message = "Không tìm thấy dữ liệu";
                return response;
            }
            _imageDetailRepository.DeleteImageDetailByImageIdAsync(boardingHouses.Image_Id); // lấy giá trị lớn nhất trong bảng hình ảnh

            var imageDetailMax = _imageDetailRepository.GetMax(); // lấy giá trị lớn nhất trong bảng hình ảnh
            int imageDetailIdMax = imageDetailMax.Result.Id;

            List<ImagesDetails> listImageDetail = JsonConvert.DeserializeObject<List<ImagesDetails>>(model.Json_Detail + "")!;
            listImageDetail.ForEach(async (item) =>
            {
                item.Date_Create = DateTime.Now;
                item.Image_Id = boardingHouses.Image_Id;
                item.Id = ++imageDetailIdMax;
                await _imageDetailRepository.Add(item);
            });
            boardingHousesEntity.Qty = boardingHouses.Qty;
            boardingHousesEntity.Adddress = boardingHouses.Adddress;
            boardingHousesEntity.Name = boardingHouses.Name;
            boardingHousesEntity.Ward_Id = boardingHouses.Ward_Id;
            boardingHousesEntity.Image_Id = boardingHouses.Image_Id;
            boardingHousesEntity.Date_Update = DateTime.Now;
            //boardingHousesEntity.User_Update = model.UserId;
            _boardinghousesRepository.Update(boardingHousesEntity);
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


    }
}
