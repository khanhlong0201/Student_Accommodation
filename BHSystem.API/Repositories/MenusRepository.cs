using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IMenusRepository : IGenericRepository<Menus>
    {
        Task<IEnumerable<Menus>> GetMenuByRoleAsync(int pRoleId);
    }
    public class MenusRepository : GenericRepository<Menus>, IMenusRepository
    {
        public MenusRepository(ApplicationDbContext context) : base(context){ }

        public async Task<IEnumerable<Menus>> GetMenuByRoleAsync(int pRoleId)
        {
            var results = await (from r in _context.RoleMenus
                                 join u in _dbSet on r.Menu_Id equals u.MenuId
                                 where r.Role_Id == pRoleId
                                 select u).ToListAsync();
            return results;
        }
    }
}
