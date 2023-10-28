using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IMenuRepository : IGenericRepository<Menus>
    {
    }
    public class MenuRepository : GenericRepository<Menus>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context) : base(context){ }

    }
}
