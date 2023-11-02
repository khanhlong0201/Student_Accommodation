using BHSystem.API.Infrastructure;
using BHSytem.Models.Models;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BHSystem.API.Repositories
{
    public interface IDistinctsRepository : IGenericRepository<Distincts>
    {
        //Task<IEnumerable<Distincts>> GetAll();
        //Task<Distincts> GetId(int id);
        // Task<IEnumerable<DistinctModel>> GetAllAsync();
        Task<IEnumerable<DistinctModel>> GetAllByCityAsync(int city_id);
    }
    public class DistinctsRepository : GenericRepository<Distincts>, IDistinctsRepository
    {
        public DistinctsRepository(ApplicationDbContext context) : base(context){ }
        //public async Task<IEnumerable<Distincts>> GetAll()
        //{
        //    var result = await _dbSet.ToListAsync();
        //    ////return await _dbSet.Include(d => d.City).ToListAsync();
        //    ////return result;
        //    //var result = await _context.Distincts
        //    //.FromSqlRaw("SELECT d.*, c.Name as City_Name " +
        //    //             "FROM Distincts d " +
        //    //             "INNER JOIN Citys c ON d.City_Id = c.Id")
        //    //.ToListAsync();

        //    //var mappedResult = _mapper.Map<IEnumerable<DistinctModel >>(result);
        //    //return mappedResult;
        //    return result;
        //}

        //public async Task<IEnumerable<DistinctModel>> GetAllAsync()
        //{
        //    var result = await (from d in _context.Distincts
        //                  join c in _context.Citys on d.City_Id equals c.Id
        //                  select new DistinctModel()
        //                  {
        //                      City_Name = c.Name
        //                  }).ToListAsync();
        //    return result;
        //}

        public async Task<IEnumerable<DistinctModel>> GetAllByCityAsync(int city_id)
        {
            var result = await (from d in _context.Distincts
                                where d.City_Id == city_id
                                select new DistinctModel()
                                {
                                    Id = d.Id,
                                    Name = d.Name
                                }).ToListAsync();
            return result;
        }

    }
}
