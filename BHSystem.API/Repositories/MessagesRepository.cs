using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BHSystem.API.Repositories
{
    public interface IMessagesRepository : IGenericRepository<Messages>
    {
        Task<IEnumerable<MessageModel>> GetUnReadMessageByUser(int pUserId, bool pIsAll);
    }
    public class MessagesRepository : GenericRepository<Messages>, IMessagesRepository
    {
        public MessagesRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// lấy những tin nhắn chưa đọc theo user/ hoặc tất cả
        /// </summary>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MessageModel>> GetUnReadMessageByUser(int pUserId, bool pIsAll)
        {
            var result = await (from t0 in _dbSet
                                join t1 in _context.UserMessages on t0.Id equals t1.Message_Id
                                where (pIsAll == true ||(t1.IsReaded == false && pIsAll == false)) && t1.UserId == pUserId
                                select new MessageModel()
                                {
                                    Id = t0.Id,
                                    Message = t0.Message,
                                    UserId = t1.UserId,
                                    JText = t0.JText,
                                    IsReaded = t1.IsReaded,
                                    Type = t0.Type,
                                    Date_Create = t0.Date_Create,
                                    Date_Update = t0.Date_Update,
                                    IsDeleted = t0.IsDeleted,
                                    User_Create = t0.User_Create,
                                    User_Update = t0.User_Update,
                                })
                                .OrderByDescending(m=>m.Date_Create).ToListAsync();
            return result;
        }
    }
}
