using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagement_BackEnd.Application.DTO;
using HotelManagement_BackEnd.Application.Interfaces;
using HotelManagement_BackEnd.Model;
using Microsoft.Data.SqlClient;

namespace HotelManagement_BackEnd.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IDbConnection _db;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(ILogger<ServiceService> logger, IDbConnection db)
        {

            _logger = logger;
            _db = db;
        }




        // Tạo mới và trả về object Service đã tạo
        public async Task<Service> CreateServiceAsync(ServiceDTO model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TenDichVu", model.TenDichVu);
                parameters.Add("@MaLoaiDV", model.MaLoaiDV);
                parameters.Add("@Gia", model.Gia);

                // Stored procedure cần SELECT trả về tất cả cột của Service sau khi insert
                var created = await _db.QuerySingleAsync<Service>(
                    "sp_CreateService",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service");
                throw new ApplicationException("Error creating service: " + ex.Message);
            }
        }
        public async Task<Service> UpdateServiceAsync(int id,ServiceDTO model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaDichVu", id);
                parameters.Add("@TenDichVu", model.TenDichVu);
                parameters.Add("@MaLoaiDV", model.MaLoaiDV);
                parameters.Add("@Gia", model.Gia);

                // Stored procedure cần SELECT trả về tất cả cột của Service sau khi update
                var updated = await _db.QuerySingleAsync<Service>(
                    "sp_UpdateService",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating service with ID {id}");
                throw new ApplicationException($"Error updating service: {ex.Message}");
            }
        }


        public async Task<bool> DeleteServiceAsync(int id)
        {
            try
            {

                var parameters = new { MaDichVu = id };
                var affectedRows = await _db.ExecuteAsync("sp_DeleteService", parameters, commandType: CommandType.StoredProcedure);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting service with ID {id}");
                throw;
            }
        }

        // Lấy Service theo ID
        public async Task<Service> GetServiceByIdAsync(int id)
        {
            try
            {
                var parameters = new { MaDichVu = id };
                var service = await _db.QueryFirstOrDefaultAsync<Service>(
                    "sp_GetServiceById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return service;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting service with ID {id}");
                throw;
            }
        }

        public async Task<PaginatedResponse<Service>> GetAllServicesAsync(PaginationParams parameters)
        {
            try
            {
                using var multi = await _db.QueryMultipleAsync(
                    "sp_GetAllServices",
                    new
                    {
                        PageNumber = parameters.PageNumber,
                        PageSize = parameters.PageSize,
                        SearchTerm = parameters.SearchTerm ?? (object)DBNull.Value
                    },
                    commandType: CommandType.StoredProcedure
                );

                var services = await multi.ReadAsync<Service>();
                var totalCount = await multi.ReadSingleAsync<int>();

                return new PaginatedResponse<Service>
                {
                    Items = services.ToList(),
                    TotalCount = totalCount,
                    PageNumber = parameters.PageNumber,
                    PageSize = parameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all services");
                throw new ApplicationException($"Error getting all services: {ex.Message}");
            }
        }
    }
}