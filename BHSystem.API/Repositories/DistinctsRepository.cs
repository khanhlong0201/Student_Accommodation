using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IDistinctsRepository : IGenericRepository<Distincts>
    {
    }
    public class DistinctsRepository : GenericRepository<Distincts>, IDistinctsRepository
    {
        public DistinctsRepository(ApplicationDbContext context) : base(context){ }

    }
}
