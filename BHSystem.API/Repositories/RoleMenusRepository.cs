using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRoleMenusRepository : IGenericRepository<RoleMenus>
    {
    }
    public class RoleMenusRepository : GenericRepository<RoleMenus>, IRoleMenusRepository
    {
        public RoleMenusRepository(ApplicationDbContext context) : base(context){ }

    }
}
