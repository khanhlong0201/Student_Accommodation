using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IMenusRepository : IGenericRepository<Menus>
    {
    }
    public class MenusRepository : GenericRepository<Menus>, IMenusRepository
    {
        public MenusRepository(ApplicationDbContext context) : base(context){ }

    }
}
