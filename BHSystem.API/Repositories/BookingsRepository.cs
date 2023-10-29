using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IBookingsRepository : IGenericRepository<Bookings>
    {
    }
    public class BookingsRepository : GenericRepository<Bookings>, IBookingsRepository
    {
        public BookingsRepository(ApplicationDbContext context) : base(context){ }

    }
}
