using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using BHSytem.Models.Models;
namespace BHSystem.API.Repositories
{
    public interface IImagesDetailsRepository : IGenericRepository<ImagesDetails>
    {
        Task<ImagesDetails> GetMax();
        Task<IEnumerable<ImagesDetailModel>> GetImageDetailByImageIdAsync(int imageId);
    }
    public class ImagesDetailsRepository : GenericRepository<ImagesDetails>, IImagesDetailsRepository
    {
        public ImagesDetailsRepository(ApplicationDbContext context) : base(context){ }
        public async Task<ImagesDetails> GetMax()
        {
            var maxId = await _dbSet?.MaxAsync(image => image.Id);
            return await _dbSet?.FirstOrDefaultAsync(image => image.Id == maxId);
        }

        public async Task<IEnumerable<ImagesDetailModel>> GetImageDetailByImageIdAsync(int imageId)
        {
            var results = await (from r in _context.Images
                                 join u in _dbSet on r.Id equals u.Image_Id
                                 where r.Id == imageId
                                 select new ImagesDetailModel()
                                 {
                                     Id = u.Id,
                                     Image_Id = u.Image_Id,
                                     File_Path = u.File_Path,
                                     ImageUrl = u.File_Path,
                                 }).ToListAsync();
            return results;
        }
    }
}
