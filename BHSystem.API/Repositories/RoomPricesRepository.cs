using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRoomPricesRepository : IGenericRepository<RoomPrices>
    {
        Task<IEnumerable<RoomPriceModel>> GetAllAsync();
        Task<RoomPrices> GetMax();
    }
    public class RoomPricesRepository : GenericRepository<RoomPrices>, IRoomPricesRepository
    {
        public RoomPricesRepository(ApplicationDbContext context) : base(context){}

        public async Task<IEnumerable<RoomPriceModel>> GetAllAsync()
        {
            var result = await (from d in _context.RoomPrices
                                join c in _context.Rooms on d.Room_Id equals c.Id
                                join b in _context.BoardingHouses on c.Boarding_House_Id equals b.Id
                                where d.IsDeleted == false
                                select new RoomPriceModel()
                                {
                                    Id = d.Id,
                                    Room_Id = d.Room_Id,
                                    Price = d.Price,
                                    Room_Name = c.Name,
                                    BoardingHouse_Id = c.Boarding_House_Id,
                                    BoardingHouse_Name = b.Name,
                                }).ToListAsync();
            return result;
        }

        public async Task<RoomPrices> GetMax()
        {
            var maxId = await _dbSet.MaxAsync(roomPrice => (int?)roomPrice.Id);

            if (maxId == null)
            {
                return null; // Không tìm thấy bất kỳ đối tượng nào
            }

            var roomPriceMax = await _dbSet.FirstOrDefaultAsync(roomPrice => roomPrice.Id == maxId);
            return roomPriceMax;
        }
    }
}
