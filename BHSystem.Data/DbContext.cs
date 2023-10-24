using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHSystem.Data
{
    public interface IDbContext
    {
        Task Connect();
        Task DisConnect();
        Task<IEnumerable<T>> GetDataAsync<T>(string commandText, Func<IDataReader, T> dataFunc, IEnumerable<SqlParameter>? sqlParameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetDataByIdAsync<T>(string commandText, Func<IDataReader, T> dataFunc, IEnumerable<SqlParameter>? sqlParameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<DataSet> GetDataSetAsync(string commandText, IEnumerable<SqlParameter>? sqlParameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<DataTable> AddOrUpdateAsync(string commandText, IEnumerable<SqlParameter> sqlParameters, CommandType commandType = CommandType.StoredProcedure);
    }
    public class DbContext : IDbContext
    {
        public Task<DataTable> AddOrUpdateAsync(string commandText, IEnumerable<SqlParameter> sqlParameters, CommandType commandType = CommandType.StoredProcedure)
        {
            throw new NotImplementedException();
        }

        public Task Connect()
        {
            throw new NotImplementedException();
        }

        public Task DisConnect()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetDataAsync<T>(string commandText, Func<IDataReader, T> dataFunc, IEnumerable<SqlParameter>? sqlParameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetDataByIdAsync<T>(string commandText, Func<IDataReader, T> dataFunc, IEnumerable<SqlParameter>? sqlParameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            throw new NotImplementedException();
        }

        public Task<DataSet> GetDataSetAsync(string commandText, IEnumerable<SqlParameter>? sqlParameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            throw new NotImplementedException();
        }
    }
}
