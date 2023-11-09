using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRoomsRepository : IGenericRepository<Rooms>
    {
        Task<IEnumerable<RoomModel>> GetAllByBHouseAsync(int city_id);
    }
    public class RoomsRepository : GenericRepository<Rooms>, IRoomsRepository
    {
        public RoomsRepository(ApplicationDbContext context) : base(context){ }
        public async Task<IEnumerable<RoomModel>> GetAllByBHouseAsync(int bHouse_id)
        {
            var result = await (from d in _context.Rooms
                                where d.Boarding_House_Id == bHouse_id
                                select new RoomModel()
                                {
                                    Id = d.Id,
                                    Name = d.Name
                                }).ToListAsync();
            return result;
        }

    }
}
