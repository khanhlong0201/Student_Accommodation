using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<Users?> LoginAsync(Users request);
    }
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context){ }

        public async Task<Users?> LoginAsync(Users request) => await _dbSet.FirstOrDefaultAsync(m => m.UserName == request.UserName && m.Password == m.Password);
    }
}
