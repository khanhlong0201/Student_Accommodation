using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;

namespace BHSystem.API.Repositories
{
    public interface IMessagesRepository : IGenericRepository<Messages>
    {

    }
    public class MessagesRepository : GenericRepository<Messages>, IMessagesRepository
    {
        public MessagesRepository(ApplicationDbContext context) : base(context) { }
    }
}
