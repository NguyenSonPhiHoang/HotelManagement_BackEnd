using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement_BackEnd.Application.DTO;
using HotelManagement_BackEnd.Model;

namespace HotelManagement_BackEnd.Application.Interfaces
{
    public interface IServiceTypeService
    {
        Task<ServiceType> CreateServiceTypeAsync(ServiceTypeDTO model);
        Task<ServiceType> UpdateServiceTypeAsync(int id,ServiceTypeDTO model);
        Task<bool> DeleteServiceTypeAsync(int id);
        Task<ServiceType> GetServiceTypeByIdAsync(int id);
        Task<PaginatedResponse<ServiceType>> GetAllServiceTypesAsync(PaginationParams parameters);
    }
}