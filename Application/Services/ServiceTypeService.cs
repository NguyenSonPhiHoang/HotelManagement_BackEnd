using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagement_BackEnd.Application.DTO;
using HotelManagement_BackEnd.Application.Interfaces;
using HotelManagement_BackEnd.Model;

namespace HotelManagement_BackEnd.Application.Services
{
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<ServiceTypeService> _logger;

        public ServiceTypeService(IDbConnection connection, ILogger<ServiceTypeService> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        public async Task<ServiceType> CreateServiceTypeAsync(ServiceTypeDTO model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TenLoaiDV", model.TenLoaiDV);
                parameters.Add("@MoTa", model.MoTa);

                // Thực thi stored procedure và lấy giá trị Id trả về
                var result = await _connection.QuerySingleAsync<ServiceType>(
                    "sp_CreateServiceType",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service type");
                throw new ApplicationException("Error creating service type: " + ex.Message);
            }
        }

        public async Task<ServiceType> UpdateServiceTypeAsync(int id,ServiceTypeDTO model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaLoaiDV", id);
                parameters.Add("@TenLoaiDV", model.TenLoaiDV);
                parameters.Add("@MoTa", model.MoTa);

                // Giả sử stored procedure trả về bản ghi ServiceType vừa cập nhật
                return await _connection.QuerySingleAsync<ServiceType>(
                    "sp_UpdateServiceType",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating service type with ID {id}");
                throw new ApplicationException($"Error updating service type: {ex.Message}");
            }
        }

        public async Task<bool> DeleteServiceTypeAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaLoaiDV", id);

                // ExecuteAsync trả về số rows bị xóa
                var affectedRows = await _connection.ExecuteAsync(
                    "sp_DeleteServiceType",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // > 0 nghĩa là có xóa được ít nhất 1 bản ghi
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting service type with ID {id}");
                throw new ApplicationException($"Error deleting service type: {ex.Message}");
            }
        }


        public async Task<ServiceType> GetServiceTypeByIdAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaLoaiDV", id);

                // Trả về ServiceType theo ID
                return await _connection.QuerySingleOrDefaultAsync<ServiceType>(
                    "sp_GetServiceTypeById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting service type with ID {id}");
                throw new ApplicationException($"Error getting service type: {ex.Message}");
            }
        }

        public async Task<PaginatedResponse<ServiceType>> GetAllServiceTypesAsync(PaginationParams parameters)
        {
            try
            {
                var dynParams = new DynamicParameters();
                dynParams.Add("@PageNumber", parameters.PageNumber);
                dynParams.Add("@PageSize", parameters.PageSize);
                dynParams.Add("@SearchTerm", parameters.SearchTerm ?? (object)DBNull.Value);

                using (var multi = await _connection.QueryMultipleAsync(
                    "sp_GetAllServiceTypes",
                    dynParams,
                    commandType: CommandType.StoredProcedure))
                {
                    var items = (await multi.ReadAsync<ServiceType>()).ToList();
                    var total = await multi.ReadSingleAsync<int>();

                    return new PaginatedResponse<ServiceType>
                    {
                        Items = items,
                        TotalCount = total,
                        PageNumber = parameters.PageNumber,
                        PageSize = parameters.PageSize
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all service types");
                throw new ApplicationException($"Error getting all service types: {ex.Message}");
            }
        }
    }
}