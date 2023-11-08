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
    }
}
