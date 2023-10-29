using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IUserRolesRepository : IGenericRepository<UserRoles>
    {
    }
    public class UserRolesRepository : GenericRepository<UserRoles>, IUserRolesRepository
    {
        public UserRolesRepository(ApplicationDbContext context) : base(context){ }

    }
}
