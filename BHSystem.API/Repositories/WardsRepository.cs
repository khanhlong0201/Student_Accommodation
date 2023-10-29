using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IWardsRepository : IGenericRepository<Wards>
    {
    }
    public class WardsRepository : GenericRepository<Wards>, IWardsRepository
    {
        public WardsRepository(ApplicationDbContext context) : base(context){ }

    }
}
