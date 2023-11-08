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
        Task<bool> DeleteImageDetailByImageIdAsync(int imageId);
        Task<bool> DeleteById(int id);
    }
    public class ImagesDetailsRepository : GenericRepository<ImagesDetails>, IImagesDetailsRepository
    {
        public ImagesDetailsRepository(ApplicationDbContext context) : base(context) { }
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

        public async Task<bool> DeleteImageDetailByImageIdAsync(int imageId)
        {
            try
            {
                var childEntities = await _dbSet.Where(c => c.Image_Id == imageId) // Lọc theo cột khóa ngoại
                    .ToListAsync();
                _dbSet.RemoveRange(childEntities); // Xóa dữ liệu con
                return true;
            }
            catch (Exception)
            {
                // Xử lý lỗi nếu cần thiết
                return false;
            }
        }

        public async Task<bool> DeleteById(int id)
        {
            try
            {
                var entityToDelete = await _dbSet.FirstOrDefaultAsync(c => c.Id == id);

                if (entityToDelete != null)
                {
                    _dbSet.Remove(entityToDelete);
                    return true;
                }
                else
                {
                    return false; // Không tìm thấy bản ghi cần xóa
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần thiết
                return false;
            }
        }
    }
}
