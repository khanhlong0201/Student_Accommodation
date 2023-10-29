using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IImagesDetailsRepository : IGenericRepository<ImagesDetails>
    {
    }
    public class ImagesDetailsRepository : GenericRepository<ImagesDetails>, IImagesDetailsRepository
    {
        public ImagesDetailsRepository(ApplicationDbContext context) : base(context){ }

    }
}
