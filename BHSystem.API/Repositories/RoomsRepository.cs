using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRoomsRepository : IGenericRepository<Rooms>
    {
    }
    public class RoomsRepository : GenericRepository<Rooms>, IRoomsRepository
    {
        public RoomsRepository(ApplicationDbContext context) : base(context){ }

    }
}
