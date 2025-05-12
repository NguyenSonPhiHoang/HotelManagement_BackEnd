using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HotelManagement.DataReader
{
    public class DatabaseDapper
    {
        private readonly string connectionString = "Server=118.69.126.49;Initial Catalog=Data_QLKS_112_Nhom2;User ID=user_112_nhom2;Password=123456789;Encrypt=True;Trust Server Certificate=True";
        private readonly SqlConnection conn;
        private const int DefaultCommandTimeout = 30; // Timeout mặc định 30 giây

        public DatabaseDapper()
        {
            try
            {
                conn = new SqlConnection(connectionString);
            }
            catch (Exception)
            {
                throw; // Giữ nguyên stack trace
            }
        }

        // Cho phép truy vấn dữ liệu với stored procedure (sync)
        public IEnumerable<T> QueryStoredProcedure<T>(string storedProcedure, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return conn.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Cho phép truy vấn dữ liệu với stored procedure (async)
        public async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string storedProcedure, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();
                return await conn.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Cho phép truy vấn dữ liệu với câu SQL trực tiếp (sync)
        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return conn.Query<T>(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Cho phép truy vấn dữ liệu với câu SQL trực tiếp (async)
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();
                return await conn.QueryAsync<T>(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Lấy dòng đầu tiên hoặc mặc định với stored procedure (sync)
        public T QueryFirstOrDefaultStoredProcedure<T>(string storedProcedure, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return conn.QueryFirstOrDefault<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Lấy dòng đầu tiên hoặc mặc định với stored procedure (async)
        public async Task<T> QueryFirstOrDefaultStoredProcedureAsync<T>(string storedProcedure, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Lấy dòng đầu tiên hoặc mặc định với câu SQL (sync)
        public T QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return conn.QueryFirstOrDefault<T>(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Lấy dòng đầu tiên hoặc mặc định với câu SQL (async)
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<T>(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Thực thi câu lệnh SQL không truy vấn với stored procedure (sync)
        public int ExecuteStoredProcedure(string storedProcedure, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return conn.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Thực thi câu lệnh SQL không truy vấn với stored procedure (async)
        public async Task<int> ExecuteStoredProcedureAsync(string storedProcedure, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();
                return await conn.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Thực thi câu lệnh SQL không truy vấn (sync)
        public int Execute(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return conn.Execute(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Thực thi câu lệnh SQL không truy vấn (async)
        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();
                return await conn.ExecuteAsync(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Thực thi câu lệnh SQL và trả về giá trị vô hướng (sync)
        public T ExecuteScalar<T>(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return conn.ExecuteScalar<T>(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Thực thi câu lệnh SQL và trả về giá trị vô hướng (async)
        public async Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null)
        {
            using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();
                return await conn.ExecuteScalarAsync<T>(sql, parameters, commandTimeout: DefaultCommandTimeout);
            }
        }

        // Thực thi multiple queries và trả về nhiều result sets (sync)
        public SqlMapper.GridReader QueryMultiple(string sql, object parameters = null)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return SqlMapper.QueryMultiple(conn, sql, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
        }

        // Thực thi multiple queries và trả về nhiều result sets (async)
        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object parameters = null)
        {
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();
            return await SqlMapper.QueryMultipleAsync(conn, sql, parameters, commandType: CommandType.StoredProcedure, commandTimeout: DefaultCommandTimeout);
        }
    }
}