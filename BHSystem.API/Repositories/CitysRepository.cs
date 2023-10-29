using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface ICitysRepository : IGenericRepository<Citys>
    {
    }
    public class CitysRepository : GenericRepository<Citys>, ICitysRepository
    {
        public CitysRepository(ApplicationDbContext context) : base(context){ }

    }
}
