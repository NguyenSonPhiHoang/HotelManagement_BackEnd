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
        public int LoaiPhong { get; set; }
        public double GiaPhong { get; set; }
        public string TrangThai { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public string TenLoaiPhong { get; set; }
    }

    public class AddRoom
    {
        public string SoPhong { get; set; }
        public int LoaiPhong { get; set; }
        public double GiaPhong { get; set; }
        public string TrangThai { get; set; }
    }
}