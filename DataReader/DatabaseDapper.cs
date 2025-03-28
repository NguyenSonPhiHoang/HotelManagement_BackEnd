using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HotelManagement.DataReader
{
    public class DatabaseDapper
    {
        private string connectionString = "Server=SINDYZ;Initial Catalog=HotelManagement;Integrated Security=True; TrustServerCertificate=True;";
        private SqlConnection conn;
        private DataTable dt;
        private SqlCommand cmd;

        public DatabaseDapper()
        {
            try
            {
                conn = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Cho phép truy vấn dữ liệu với stored procedure
        public IEnumerable<T> QueryStoredProcedure<T>(string storedProcedure, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Cho phép truy vấn dữ liệu với câu SQL trực tiếp
        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.Query<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Lấy dòng đầu tiên hoặc mặc định với stored procedure
        public T QueryFirstOrDefaultStoredProcedure<T>(string storedProcedure, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.QueryFirstOrDefault<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Lấy dòng đầu tiên hoặc mặc định với câu SQL
        public T QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.QueryFirstOrDefault<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Thực thi câu lệnh SQL không truy vấn với stored procedure
        public int ExecuteStoredProcedure(string storedProcedure, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Thực thi câu lệnh SQL không truy vấn
        public int Execute(string sql, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.Execute(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Thực thi câu lệnh SQL và trả về giá trị vô hướng
        public T ExecuteScalar<T>(string sql, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.ExecuteScalar<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Thực thi multiple queries và trả về nhiều result sets
        public SqlMapper.GridReader QueryMultiple(string sql, object parameters = null)
        {
            try
            {
                conn.Open();
                return conn.QueryMultiple(sql, parameters);
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
        }
    }
}