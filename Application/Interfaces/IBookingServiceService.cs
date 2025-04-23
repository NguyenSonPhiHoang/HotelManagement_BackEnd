using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement_BackEnd.Model;

namespace HotelManagement_BackEnd.Application.Interfaces
{
    public interface IBookingServiceService
    {
        Task<int> CreateBookingServiceAsync(BookingService model);
        Task UpdateBookingServiceAsync(BookingService model);
        Task DeleteBookingServiceAsync(int id);
        Task<BookingService> GetBookingServiceByIdAsync(int id);
        Task<PaginatedResponse<BookingService>> GetAllBookingServicesAsync(PaginationParams parameters);
    }
}