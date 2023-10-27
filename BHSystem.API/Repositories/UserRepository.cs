using BHSystem.API.Infrastructure;
using BHSytem.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IUserRepository : IGenericRepository<UserModel>
    {
        Task<UserModel?> LoginAsync(UserModel request);
    }
    public class UserRepository : GenericRepository<UserModel>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context){ }

        public async Task<UserModel?> LoginAsync(UserModel request) => await _dbSet.FirstOrDefaultAsync(m => m.UserName == request.UserName && m.Password == m.Password);
    }
}
