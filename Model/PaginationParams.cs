using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement_BackEnd.Model
{
    public class PaginationParams
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value < 1) ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value < 1 || value > 100) ? 10 : value;
        }

        public string? SearchTerm { get; set; } = string.Empty;
    }
}