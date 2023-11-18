using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRoomsRepository : IGenericRepository<Rooms>
    {
        Task<IEnumerable<RoomModel>> GetAllByBHouseAsync(int city_id);
        Task<IEnumerable<RoomModel>> GetAllByStatusAsync(string type);
        Task<IEnumerable<RoomModel>> GetDataByBHouseAsync(int iBHouseId);
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

        public async Task<IEnumerable<RoomModel>> GetAllByStatusAsync(string type)
        {
            var result = await (from a in _context.Rooms
                                join b in _context.BoardingHouses on a.Boarding_House_Id equals b.Id
                                join c in _context.Images on a.Image_Id equals c.Id
                                from d in _context.ImagesDetails.Where(d => d.Image_Id == c.Id).Take(1) //inner join select top 1
                                where a.IsDeleted == false && (type + "" == "" || a.Status == type)
                                select new RoomModel()
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    BHouseId = b.Id,
                                    BHouseName = b.Name,
                                    Image_Id = a.Image_Id,
                                    Date_Create = a.Date_Create,
                                    Date_Update = a.Date_Update,
                                    Status = a.Status,
                                    File_Path= d.File_Path
                                }).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<RoomModel>> GetDataByBHouseAsync(int iBHouseId)
        {
            var result = await (from a in _dbSet
                                join b in _context.BoardingHouses on a.Boarding_House_Id equals b.Id
                                join u in _context.Users on b.User_Id equals u.UserId
                                where a.IsDeleted == false && a.Boarding_House_Id == iBHouseId
                                select new RoomModel()
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    BHouseId = b.Id,
                                    BHouseName = b.Name,
                                    Status = a.Status,
                                    Address = a.Address,
                                    Length = a.Length,
                                    Width = a.Width,
                                    Price = a.Price,
                                    Image_Id = a.Image_Id,
                                    Description = a.Description,
                                    Date_Create = a.Date_Create,
                                    User_Create = a.User_Create,
                                    Date_Update = a.Date_Update,
                                    User_Update = a.User_Update,
                                    UserName = u.FullName,
                                    Phone = u.Phone
                                }).ToListAsync();
            return result;
        }
    }
}
