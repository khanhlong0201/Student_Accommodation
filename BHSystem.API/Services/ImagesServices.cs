using BHSystem.API.Common;
using BHSystem.API.Infrastructure;
using BHSystem.API.Repositories;
using BHSytem.Models.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace BHSystem.API.Services
{
    public interface IImagesService
    {
        Task<IEnumerable<Images>> GetDataAsync();
    }
    public class ImagesService : IImagesService
    {
        private readonly IImagesRepository _imagesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ImagesService(IImagesRepository imagesRepository, IUnitOfWork unitOfWork)
        {
            _imagesRepository = imagesRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Images>> GetDataAsync() => await _imagesRepository.GetAll();
    }
}
