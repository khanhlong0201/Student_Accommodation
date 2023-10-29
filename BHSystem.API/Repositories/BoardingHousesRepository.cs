using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IBoardingHousesRepository : IGenericRepository<BoardingHouses>
    {
    }
    public class BoardingHousesRepository : GenericRepository<BoardingHouses>, IBoardingHousesRepository
    {
        public BoardingHousesRepository(ApplicationDbContext context) : base(context){ }

    }
}
