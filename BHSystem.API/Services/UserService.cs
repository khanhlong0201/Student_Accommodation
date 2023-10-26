using BHSystem.API.Common;
using BHSytem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;

namespace BHSystem.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetDataAsync();
        Task<ResponseModel> UpdateUserAsync(RequestModel request);
        Task<ResponseModel<UserModel>> LoginAsync(UserModel requestLogin);
    }
    public class UserService : IUserService
    {
        private readonly IBHouseDbContext _context;
        public UserService(IBHouseDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// đăng nhập
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UserModel>> LoginAsync(UserModel loginRequest)
        {
            ResponseModel<UserModel> response = new();
            try
            {
                await _context.Connect();
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@UserName", loginRequest.UserName);
                sqlParameters[1] = new SqlParameter("@PassWord", loginRequest.Password);
                //sqlParameters[2] = new SqlParameter("@RefreshToken", loginRequest.RefreshToken);
                response = await _context.GetDataByIdAsync(Constants.STORE_USER_LOGIN, DataRecordToResponseModel, sqlParameters);
                // check error message sql server  -> set status
                if (response != null && response.StatusCode == -1) response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                Trace.TraceError("LoginAsync", ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            finally
            {
                await _context.DisConnect();
            }
            return response;
        }

        /// <summary>
        /// lấy danh sách User
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserModel>> GetDataAsync()
        {
            IEnumerable<UserModel> data;
            try
            {
                await _context.Connect();
                data = await _context.GetDataAsync(@"Select * from dbo.[USER]", DataRecordToUserModel, commandType: CommandType.Text);
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

        /// <summary>
        /// cập nhật thông tin user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateUserAsync(RequestModel request)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                await _context.Connect();
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@UserId", request.UserId);
                sqlParameters[1] = new SqlParameter("@Json", request.Json);
                sqlParameters[2] = new SqlParameter("@Type", request.Type);
                var data = await _context.AddOrUpdateAsync(Constants.STORE_USER_UPDATE, sqlParameters);
                if (data != null && data.Rows.Count > 0)
                {
                    response.Status = int.Parse(data.Rows[0]["Status"]?.ToString() ?? "-1");
                    response.Message = data.Rows[0]["Message"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("UpdateDataAsync", ex.Message);
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
            }
            finally
            {
                await _context.DisConnect();
            }
            return response;
        }

        /// <summary>
        /// đọc từng record từ câu select
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private UserModel DataRecordToUserModel(IDataRecord record)
        {
            UserModel user = new();
            if (!Convert.IsDBNull(record["UserId"])) user.UserId = Convert.ToInt32(record["USERID"] + "");
            if (!Convert.IsDBNull(record["UserName"])) user.UserName = Convert.ToString(record["UserName"]);
            if (!Convert.IsDBNull(record["Password"])) user.Password = Convert.ToString(record["Password"]);
            if (!Convert.IsDBNull(record["PasswordReset"])) user.PasswordReset = Convert.ToString(record["PasswordReset"]);
            if (!Convert.IsDBNull(record["FullName"])) user.FullName = Convert.ToString(record["FullName"]);
            if (!Convert.IsDBNull(record["Email"])) user.Email = Convert.ToString(record["Email"]);
            if (!Convert.IsDBNull(record["Phone"])) user.Phone = Convert.ToString(record["Phone"]);
            if (!Convert.IsDBNull(record["Address"])) user.Address = Convert.ToString(record["Address"]);
            if (!Convert.IsDBNull(record["User_Create"])) user.User_Create = Convert.ToInt32(record["User_Create"]);
            if (!Convert.IsDBNull(record["Date_Create"])) user.Date_Create = Convert.ToDateTime(record["Date_Create"]);
            if (!Convert.IsDBNull(record["User_Update"])) user.User_Update = Convert.ToInt32(record["User_Update"]);
            if (!Convert.IsDBNull(record["Date_Update"])) user.Date_Update = Convert.ToDateTime(record["Date_Update"]);
            return user;
        }

        /// <summary>
        /// đọc thông tin User Login -> kèm tình trạng login dưới db
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private ResponseModel<UserModel> DataRecordToResponseModel(IDataRecord record)
        {
            ResponseModel<UserModel> user = new();
            if (!Convert.IsDBNull(record["Status"])) user.StatusCode = Convert.ToInt32(record["Status"]);
            if (!Convert.IsDBNull(record["Message"])) user.Message = Convert.ToString(record["Message"]);
            if (user.StatusCode != -1) user.Data = DataRecordToUserModel(record);
            return user;
        }
    }
}
