
using System.Data;
using System.Data.SqlClient;


namespace HotelManagement_BackEnd.Helpers
{
    public class DbHelper
    {
        private readonly string _connectionString;
        private readonly ILogger<DbHelper> _logger;

        public DbHelper(IConfiguration configuration, ILogger<DbHelper> logger)
        {
            _connectionString = configuration.GetConnectionString("Dbcontext");
            _logger = logger;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<T> ExecuteScalarAsync<T>(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }

        public async Task ExecuteNonQueryAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(storedProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, $"SQL Error executing stored procedure: {storedProcedure}");
                    throw new ApplicationException($"Database error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error executing stored procedure: {storedProcedure}");
                    throw new ApplicationException($"Error: {ex.Message}");
                }
            }
        }

        public async Task<DataSet> ExecuteDataSetAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    await connection.OpenAsync();
                    var adapter = new SqlDataAdapter(command);
                    var ds = new DataSet();
                    adapter.Fill(ds);
                    return ds;
                }
            }
        }

        public async Task<DataTable> ExecuteDataTableAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            var dataSet = await ExecuteDataSetAsync(storedProcedure, parameters);
            return dataSet.Tables[0];
        }
    }
}