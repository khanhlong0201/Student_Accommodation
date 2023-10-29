using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IUsersRepository : IGenericRepository<Users>
    {
        Task<Users?> LoginAsync(Users request);
        Task<bool> AddUser(Users request);
    }
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        public UsersRepository(ApplicationDbContext context) : base(context){ }

        public async Task<Users?> LoginAsync(Users request) => await _dbSet.FirstOrDefaultAsync(m => m.UserName == request.UserName && m.Password == m.Password);

        public async Task<bool> AddUser(Users entity)
        {
            int userId = 1;
            if(await _dbSet.AnyAsync()) userId = await _dbSet.MaxAsync(m => m.UserId);
            entity.UserId = userId;
            await _dbSet.AddAsync(entity);
            return true;
        }    
    }
}
