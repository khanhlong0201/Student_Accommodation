using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IBHousesRepository : IGenericRepository<BoardingHouses>
    {
        Task<IEnumerable<BoardingHouseModel>> GetAllAsync();
    }
    public class BHousesRepository: GenericRepository<BoardingHouses>, IBHousesRepository
    {
        public BHousesRepository(ApplicationDbContext context) : base(context) { }
        public async Task<IEnumerable<BoardingHouseModel>> GetAllAsync()
        {
            var result = await (from d in _context.BoardingHouses
                                join c in _context.Wards on d.Ward_Id equals c.Id
                                join b in _context.Distincts on c.Distincts_Id equals b.Id
                                join a in _context.Citys on b.City_Id equals a.Id
                                join u in _context.Users on d.User_Id equals u.UserId
                                where d.IsDeleted == false
                                select new BoardingHouseModel()
                                {
                                    Id = d.Id,
                                    Name = d.Name,
                                    Adddress = d.Adddress,
                                    Image_Id = d.Image_Id,
                                    Ward_Id = d.Ward_Id,
                                    Ward_Name = c.Name,
                                    Distinct_Name = b.Name,
                                    City_Name = a.Name,
                                    City_Id = a.Id,
                                    Distinct_Id = b.Id,
                                    Qty = d.Qty,
                                    UserName = u.FullName,
                                    Phone = u.Phone
                                }).ToListAsync();
            return result;
        }
    }
}
