using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IBookingsRepository : IGenericRepository<Bookings>
    {
        Task<IEnumerable<BookingModel>> GetAllAsync(string type);
    }
    public class BookingsRepository : GenericRepository<Bookings>, IBookingsRepository
    {
        public BookingsRepository(ApplicationDbContext context) : base(context){ }
        public async Task<IEnumerable<BookingModel>> GetAllAsync(string type)
        {
            var result = await (from a in _context.Bookings
                                join b in _context.Rooms on a.Room_Id equals b.Id
                                join c in _context.Users on a.UserId equals c.UserId
                                join d in _context.BoardingHouses on b.Boarding_House_Id equals d.Id
                                where a.IsDeleted == false && (type +""=="" || a.Status == type)
                                select new BookingModel()
                                {
                                    Id = a.Id,
                                    Room_Id = a.Room_Id,
                                    Room_Name = b.Name,
                                    FullName = c.FullName== null ? a.FullName : c.FullName,
                                    Date_Create = a.Date_Create ,
                                    Date_Update = a.Date_Update,
                                    Status = a.Status,
                                    BHouse_Name = d.Name
                                }).ToListAsync();
            return result;
        }
    }
}
