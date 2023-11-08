using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IImagesRepository : IGenericRepository<Images>
    {
        Task<Images> GetMax();
    }
    public class ImagesRepository : GenericRepository<Images>, IImagesRepository
    {
        public ImagesRepository(ApplicationDbContext context) : base(context){ }

        public async Task<Images> GetMax()
        {
            var maxId = await _dbSet?.MaxAsync(image => image.Id);
            return await _dbSet?.FirstOrDefaultAsync(image => image.Id == maxId);
        }
    }
}
