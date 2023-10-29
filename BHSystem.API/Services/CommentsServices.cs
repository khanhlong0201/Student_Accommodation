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
    public interface ICommentsService
    {
        Task<IEnumerable<Comments>> GetDataAsync();
    }
    public class CommentsService : ICommentsService
    {
        private readonly ICommentsRepository _commentsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CommentsService(ICommentsRepository commentsRepository, IUnitOfWork unitOfWork)
        {
            _commentsRepository = commentsRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<Comments>> GetDataAsync() => await _commentsRepository.GetAll();
    }
}
