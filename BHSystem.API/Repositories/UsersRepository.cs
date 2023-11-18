using BHSystem.API.Infrastructure;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BHSystem.API.Repositories
{
    public interface IUsersRepository : IGenericRepository<Users>
    {
        Task<Users?> LoginAsync(Users request);
        Task<IEnumerable<Users>> GetUserByRoleAsync(int pRoleId);
        Task<IEnumerable<UserModel>> GetAllAsync();
        Task<UserModel?> GetUserById(int pUserId);
    }
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        public UsersRepository(ApplicationDbContext context) : base(context){ }

        public async Task<Users?> LoginAsync(Users request) => await _dbSet.FirstOrDefaultAsync(m => m.UserName == request.UserName 
            && m.Password == request.Password && m.IsDeleted == false);

        public async Task<IEnumerable<Users>> GetUserByRoleAsync(int pRoleId)
        {
            var results = await (from r in _context.UserRoles
                           join u in _dbSet on r.UserId equals u.UserId
                           where r.Role_Id == pRoleId && u.IsDeleted == false
                           select u).ToListAsync();
            return results;
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            var result = await (from t0 in _dbSet
                                join t1 in _context.Wards on t0.Ward_Id equals t1.Id
                                join t2 in _context.Distincts on t1.Distincts_Id equals t2.Id
                                join t3 in _context.Citys on t2.City_Id equals t3.Id
                                where t0.IsDeleted == false
                                select new UserModel()
                                {
                                    UserId= t0.UserId,
                                    FullName = t0.FullName,
                                    Address = t0.Address,
                                    Phone = t0.Phone,
                                    Email = t0.Email,
                                    UserName = t0.UserName,
                                    Password = t0.Password,
                                    PasswordReset = t0.PasswordReset,
                                    IsDeleted = t0.IsDeleted,
                                    Date_Create = t0.Date_Create,
                                    User_Create = t0.User_Create,
                                    Date_Update = t0.Date_Update,
                                    User_Update = t0.User_Update,
                                    Ward_Id = t0.Ward_Id,
                                    Ward_Name = t1.Name,
                                    Distinct_Name = t2.Name,
                                    City_Name = t3.Name,
                                    City_Id = t3.Id,
                                    Distinct_Id = t2.Id,
                                    IsAdmin = t0.Type == "Admin"
                                }).ToListAsync();
            return result;
        }

        public async Task<UserModel?> GetUserById(int pUserId)
        {
            var result = await (from t0 in _dbSet
                                join t1 in _context.Wards on t0.Ward_Id equals t1.Id
                                join t2 in _context.Distincts on t1.Distincts_Id equals t2.Id
                                join t3 in _context.Citys on t2.City_Id equals t3.Id
                                where t0.IsDeleted == false && t0.UserId == pUserId
                                select new UserModel()
                                {
                                    UserId = t0.UserId,
                                    FullName = t0.FullName,
                                    Address = t0.Address,
                                    Phone = t0.Phone,
                                    Email = t0.Email,
                                    UserName = t0.UserName,
                                    Password = t0.Password,
                                    PasswordReset = t0.PasswordReset,
                                    IsDeleted = t0.IsDeleted,
                                    Date_Create = t0.Date_Create,
                                    User_Create = t0.User_Create,
                                    Date_Update = t0.Date_Update,
                                    User_Update = t0.User_Update,
                                    Ward_Id = t0.Ward_Id,
                                    Ward_Name = t1.Name,
                                    Distinct_Name = t2.Name,
                                    City_Name = t3.Name,
                                    City_Id = t3.Id,
                                    Distinct_Id = t2.Id,
                                    IsAdmin = t0.Type == "Admin"
                                }).FirstOrDefaultAsync();
            return result;
        }
    }
}
