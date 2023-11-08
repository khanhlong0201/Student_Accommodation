using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IBoardingHousesRepository : IGenericRepository<BoardingHouses>
    {
        Task<IEnumerable<BoardingHouseModel>> GetAllAsync();
    }
    public class BoardingHousesRepository : GenericRepository<BoardingHouses>, IBoardingHousesRepository
    {
        public BoardingHousesRepository(ApplicationDbContext context) : base(context){ }
        public async Task<IEnumerable<BoardingHouseModel>> GetAllAsync()
        {
            var result = await (from d in _context.BoardingHouses
                                join c in _context.Wards on d.Ward_Id equals c.Id
                                join b in _context.Distincts on c.Distincts_Id equals b.Id
                                join a in _context.Citys on b.City_Id equals a.Id
                                join u in _context.Images on d.Image_Id equals u.Id
                                from v in _context.ImagesDetails.Where(v => v.Image_Id == u.Id).Take(1) //inner join select top 1
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
                                    File_Path = v.File_Path
                                }).ToListAsync();
            return result;
        }
    }
}
