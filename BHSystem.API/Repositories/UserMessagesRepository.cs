using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;

namespace BHSystem.API.Repositories
{
    public interface IUserMessagesRepository : IGenericRepository<UserMessages>
    {

    }
    public class UserMessagesRepository : GenericRepository<UserMessages>, IUserMessagesRepository
    {
        public UserMessagesRepository(ApplicationDbContext context) : base(context) { }
    }
}
