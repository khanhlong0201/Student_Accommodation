using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IMenusRepository : IGenericRepository<Menus>
    {
        Task<IEnumerable<Menus>> GetMenuByRoleAsync(int pRoleId);
        Task<IEnumerable<Menus>> GetMenuByUserAsync(int pUserId);
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

        public async Task<IEnumerable<Menus>> GetMenuByUserAsync(int pUserId)
        {
            List<Menus> lstMenus = new List<Menus>();
            // lấy ra role theo nhiều user
            var roleByUser = await _context.UserRoles.Where(m => m.UserId == pUserId).Select(m=>m.Role_Id).ToArrayAsync();
            if(roleByUser != null && roleByUser.Any())
            {
                // nếu nhiều role lấy tất cả các menu theo role đó -> distint đi
                var menuByRoles = await _context.RoleMenus.Where(m => roleByUser.Contains(m.Role_Id)).Select(m=>m.Menu_Id).Distinct().ToListAsync();
                lstMenus = await _dbSet.Where(m => menuByRoles.Contains(m.MenuId)).OrderBy(m => m.MenuId).ToListAsync();
            }
            return lstMenus;

        }
    }
}
