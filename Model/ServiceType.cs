using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Model
{
    public class ServiceType
    {
        public int MaLoaiDV { get; set; }
        public string TenLoaiDV { get; set; }
        public string? MoTa { get; set; }
    }

    public class AddServiceType
    {
        public string TenLoaiDV { get; set; }
        public string? MoTa { get; set; }
    }
    public class UpdateServiceType
    {
        public string? TenLoaiDV { get; set; }
        public string? MoTa { get; set; }
    }
}