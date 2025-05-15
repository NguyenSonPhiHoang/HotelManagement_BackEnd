﻿    namespace HotelManagement.Model
    {
        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }

            public static ApiResponse<T> SuccessResponse(T data, string message = "Thành công")
            {
                return new ApiResponse<T>
                {
                    Success = true,
                    Message = message,
                    Data = data
                };
            }

            public static ApiResponse<T> ErrorResponse(string message = "Lỗi")
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = message,
                    Data = default
                };
            }
        }
    }