using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagement_BackEnd.Application.Interfaces;
using HotelManagement_BackEnd.Model;
using Microsoft.Data.SqlClient;

namespace HotelManagement_BackEnd.Application.Services
{
    public class BookingServiceService : IBookingServiceService
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<BookingServiceService> _logger;

        public BookingServiceService(ILogger<BookingServiceService> logger, IDbConnection connection)
        {

            _logger = logger;
            _connection = connection;
        }

        public async Task<int> CreateBookingServiceAsync(BookingService model)
        {
            try
            {
                var parameters = new
                {
                    MaBSD = model.MaBSD,
                    MaDatPhong = model.MaDatPhong,
                    MaDichVu = model.MaDichVu,
                    SoLuong = model.SoLuong,
                    Gia = model.Gia,
                    ThanhTien = model.ThanhTien,
                    NgaySuDung = model.NgaySuDung
                };
                return await _connection.ExecuteScalarAsync<int>("sp_CreateBookingService", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking service");
                throw new ApplicationException("Error creating booking service: " + ex.Message);
            }
        }

        public async Task UpdateBookingServiceAsync(BookingService model)
        {
            try
            {
                var parameters = new
                {
                    MaBSD = model.MaBSD,
                    MaDatPhong = model.MaDatPhong,
                    MaDichVu = model.MaDichVu,
                    SoLuong = model.SoLuong,
                    Gia = model.Gia,
                    ThanhTien = model.ThanhTien,
                    NgaySuDung = model.NgaySuDung
                };
                await _connection.ExecuteAsync("sp_UpdateBookingService", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating booking service with ID {model.MaBSD}");
                throw new ApplicationException($"Error updating booking service: {ex.Message}");
            }
        }

        public async Task DeleteBookingServiceAsync(int id)
        {
            try
            {
                var parameters = new { MaBSD = id };
                await _connection.ExecuteAsync("sp_DeleteBookingService", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting booking service with ID {id}");
                throw new ApplicationException($"Error deleting booking service: {ex.Message}");
            }
        }

        public async Task<BookingService> GetBookingServiceByIdAsync(int id)
        {
            try
            {
                var parameters = new { MaBSD = id };
                return await _connection.QueryFirstOrDefaultAsync<BookingService>("sp_GetBookingServiceById", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting booking service with ID {id}");
                throw new ApplicationException($"Error getting booking service: {ex.Message}");
            }
        }

        public async Task<PaginatedResponse<BookingService>> GetAllBookingServicesAsync(PaginationParams parameters)
        {
            try
            {
                var sql = "sp_GetAllBookingServices";
                using (var multi = await _connection.QueryMultipleAsync(sql, new
                {
                    PageNumber = parameters.PageNumber,
                    PageSize = parameters.PageSize,
                    SearchTerm = parameters.SearchTerm ?? (object)DBNull.Value
                }, commandType: CommandType.StoredProcedure))
                {
                    var bookingServices = await multi.ReadAsync<BookingService>();
                    var totalCount = await multi.ReadSingleAsync<int>();

                    return new PaginatedResponse<BookingService>
                    {
                        Items = bookingServices.ToList(),
                        TotalCount = totalCount,
                        PageNumber = parameters.PageNumber,
                        PageSize = parameters.PageSize
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all booking services");
                throw new ApplicationException($"Error getting all booking services: {ex.Message}");
            }
        }
    }
}