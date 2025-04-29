using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement_BackEnd.Model
{
    public class Service
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string? TenLoaiDV { get; set; }
        public float Gia { get; set; }
    }
    public class AddService
    {
        public string TenDichVu { get; set; }
        public int? MaLoaiDV { get; set; }
        public float Gia { get; set; }
    }
    public class UpdateService
    {
        public string? TenDichVu { get; set; }
        public int? MaLoaiDV { get; set; }
        public float? Gia { get; set; }
    }
}