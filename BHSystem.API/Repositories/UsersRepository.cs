using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IUsersRepository : IGenericRepository<Users>
    {
        Task<Users?> LoginAsync(Users request);
        Task<IEnumerable<Users>> GetUserByRoleAsync(int pRoleId);
    }
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        public UsersRepository(ApplicationDbContext context) : base(context){ }

        public async Task<Users?> LoginAsync(Users request) => await _dbSet.FirstOrDefaultAsync(m => m.UserName == request.UserName && m.Password == request.Password);

        public async Task<IEnumerable<Users>> GetUserByRoleAsync(int pRoleId)
        {
            var results = await (from r in _context.UserRoles
                           join u in _dbSet on r.UserId equals u.UserId
                           where r.Role_Id == pRoleId
                           select u).ToListAsync();
            return results;
        }    
    }
}
