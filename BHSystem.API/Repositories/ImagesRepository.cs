using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IImagesRepository : IGenericRepository<Images>
    {
    }
    public class ImagesRepository : GenericRepository<Images>, IImagesRepository
    {
        public ImagesRepository(ApplicationDbContext context) : base(context){ }

    }
}
