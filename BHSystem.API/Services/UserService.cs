using BHSytem.Models;
using System.Data;

namespace BHSystem.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetDataAsync();
    }
    public class UserService : IUserService
    {
        private readonly IBHouseDbContext _context;
        public UserService(IBHouseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModel>> GetDataAsync()
        {
            IEnumerable<UserModel> data;
            try
            {
                await _context.Connect();
                data = await _context.GetDataAsync(@"Select * from dbo.[OUSER]", DataRecordToUserModel, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                await _context.DisConnect();
            }
            return data;
        }

        private UserModel DataRecordToUserModel(IDataRecord record)
        {
            UserModel user = new();
            if (!Convert.IsDBNull(record["Id"])) user.Id = Convert.ToInt32(record["Id"] + "");
            if (!Convert.IsDBNull(record["UserName"])) user.UserName = Convert.ToString(record["UserName"]);
            if (!Convert.IsDBNull(record["Password"])) user.Password = Convert.ToString(record["Password"]);
            if (!Convert.IsDBNull(record["FullName"])) user.FullName = Convert.ToString(record["FullName"]);
            if (!Convert.IsDBNull(record["Email"])) user.Email = Convert.ToString(record["Email"]);
            if (!Convert.IsDBNull(record["PhoneNumber"])) user.PhoneNumber = Convert.ToString(record["PhoneNumber"]);
            if (!Convert.IsDBNull(record["Address"])) user.Address = Convert.ToString(record["Address"]);
            if (!Convert.IsDBNull(record["UserCreated"])) user.UserCreate = Convert.ToInt32(record["UserCreated"]);
            if (!Convert.IsDBNull(record["DateCreated"])) user.DateCreate = Convert.ToDateTime(record["DateCreated"]);
            if (!Convert.IsDBNull(record["UserUpdated"])) user.UserUpdate = Convert.ToInt32(record["UserUpdated"]);
            if (!Convert.IsDBNull(record["DateUpdated"])) user.DateUpdate = Convert.ToDateTime(record["DateUpdated"]);
            return user;
        }
    }
}
