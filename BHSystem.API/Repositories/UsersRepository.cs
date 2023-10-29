using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IUsersRepository : IGenericRepository<Users>
    {
        Task<Users?> LoginAsync(Users request);
    }
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        public UsersRepository(ApplicationDbContext context) : base(context){ }

        public async Task<Users?> LoginAsync(Users request) => await _dbSet.FirstOrDefaultAsync(m => m.UserName == request.UserName && m.Password == m.Password);
    }
}
