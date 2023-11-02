using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IWardsRepository : IGenericRepository<Wards>
    {
        Task<IEnumerable<WardModel>> GetAllByDistinctAsync(int iistinct_id);
    }
    public class WardsRepository : GenericRepository<Wards>, IWardsRepository
    {
        public WardsRepository(ApplicationDbContext context) : base(context) { }
        public async Task<IEnumerable<WardModel>> GetAllByDistinctAsync(int distinct_id)
        {
            var result = await (from d in _context.Wards
                                where d.Distincts_Id == distinct_id
                                select new WardModel()
                                {
                                    Id = d.Id,
                                    Name = d.Name
                                }).ToListAsync();
            return result;
        }

    }
}
