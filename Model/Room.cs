using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelManagement_BackEnd.Model
{
    public class Room
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public string TrangThai { get; set; }
        public string TenLoaiPhong { get; set; }
        public string TinhTrang { get; set; }
    }

}