using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement_BackEnd.Application.DTO;
using HotelManagement_BackEnd.Model;

namespace HotelManagement_BackEnd.Application.Interfaces
{
    public interface IServiceService
    {
        Task<Service> CreateServiceAsync(ServiceDTO model);
        Task<Service> UpdateServiceAsync(int id, ServiceDTO model);
        Task<bool> DeleteServiceAsync(int id);
        Task<Service> GetServiceByIdAsync(int id);
        Task<PaginatedResponse<Service>> GetAllServicesAsync(PaginationParams parameters);
    }
}