using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace BHSystem.API.Services
{
    public interface IImagesDetailsService
    {
        Task<IEnumerable<ImagesDetails>> GetDataAsync();
        Task<IEnumerable<ImagesDetailModel>> GetImageDetailByImageIdAsync(int imageId);
        Task<bool> DeleteImageDetailByImageIdAsync(int imageId);
        Task<bool> DeleteById(int id);
    }
    public class ImagesDetailsService : IImagesDetailsService
    {
        private readonly IImagesDetailsRepository _imagesdetailsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ImagesDetailsService(IImagesDetailsRepository imagesdetailsRepository, IUnitOfWork unitOfWork)
        {
            _imagesdetailsRepository = imagesdetailsRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<ImagesDetails>> GetDataAsync() => await _imagesdetailsRepository.GetAll();
        public async Task<IEnumerable<ImagesDetailModel>> GetImageDetailByImageIdAsync(int imageId) => await _imagesdetailsRepository.GetImageDetailByImageIdAsync(imageId);
        public async Task<bool> DeleteImageDetailByImageIdAsync(int imageId) => await _imagesdetailsRepository.DeleteImageDetailByImageIdAsync(imageId);

        /// <summary>
        /// xóa dữ liệu thực chất cập nhật cột IsDelete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteById(int id)
        {
            var check = await _imagesdetailsRepository.DeleteById(id);
            if (!check) return false;
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
