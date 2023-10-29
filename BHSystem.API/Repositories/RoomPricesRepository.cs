using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRoomPricesRepository : IGenericRepository<RoomPrices>
    {
    }
    public class RoomPricesRepository : GenericRepository<RoomPrices>, IRoomPricesRepository
    {
        public RoomPricesRepository(ApplicationDbContext context) : base(context){ }

    }
}
