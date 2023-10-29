using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IRolesRepository : IGenericRepository<Roles>
    {
    }
    public class RolesRepository : GenericRepository<Roles>, IRolesRepository
    {
        public RolesRepository(ApplicationDbContext context) : base(context){ }

    }
}
