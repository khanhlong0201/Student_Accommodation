using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRolesRepository : IGenericRepository<Roles>
    {
        Task<Roles> GetDataByNameAsync(string name);
    }
    public class RolesRepository : GenericRepository<Roles>, IRolesRepository
    {
        public RolesRepository(ApplicationDbContext context) : base(context){ }

        public async Task<Roles> GetDataByNameAsync(string name)
        {
            var result = await (from d in _context.Roles
                                where d.IsDeleted == false && d.Name == name
                                select new Roles 
                                {
                                    Id = d.Id,
                                    Name = d.Name, 
                                }).FirstOrDefaultAsync();
            return result;
        }
    }
}
