﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HotelManagement.DataReader;
using HotelManagement.Model;
using HotelManagement.Utilities;
using System.Security.Cryptography;

namespace HotelManagement.Services
{
    public interface IAuthRepository
    {
        Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<int>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
    public class AuthRepository : IAuthRepository
    {
        private readonly DatabaseDapper _db;
        private readonly IConfiguration _configuration;
        private readonly int _accessTokenExpiryMinutes = 60; // 1 giờ
        private readonly int _refreshTokenExpiryDays = 7; // 7 ngày

        public AuthRepository(DatabaseDapper db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TenTaiKhoan) || string.IsNullOrEmpty(request.MatKhau))
                {
                    Console.WriteLine("Tên tài khoản hoặc mật khẩu trống");
                    return ApiResponse<TokenResponse>.ErrorResponse("Tên tài khoản và mật khẩu không được để trống");
                }

                var hashedPassword = PasswordHasher.HashPassword(request.MatKhau);
                var parameters = new
                {
                    TenTaiKhoan = request.TenTaiKhoan,
                    MatKhau = hashedPassword
                };

                Console.WriteLine($"Gọi sp_Login với TenTaiKhoan: {request.TenTaiKhoan}");
                var user = await _db.QueryFirstOrDefaultStoredProcedureAsync<TokenResponse>("sp_Login", parameters);
                if (user == null || user.MaTaiKhoan <= 0)
                {
                    Console.WriteLine("Không tìm thấy tài khoản hoặc mật khẩu không đúng");
                    return ApiResponse<TokenResponse>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng");
                }
                Console.WriteLine($"Tìm thấy tài khoản: MaTaiKhoan = {user.MaTaiKhoan}, TenTaiKhoan = {user.TenTaiKhoan}");

                if (string.IsNullOrEmpty(_configuration["Jwt:SecretKey"]) ||
                    string.IsNullOrEmpty(_configuration["Jwt:Issuer"]) ||
                    string.IsNullOrEmpty(_configuration["Jwt:Audience"]))
                {
                    Console.WriteLine("Cấu hình JWT bị thiếu");
                    return ApiResponse<TokenResponse>.ErrorResponse("Cấu hình JWT không hợp lệ");
                }

                var accessToken = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken(); // Sử dụng phương thức mới
                var expiresIn = _accessTokenExpiryMinutes * 60;

                Console.WriteLine($"Lưu RefreshToken cho MaTaiKhoan: {user.MaTaiKhoan}");
                await _db.ExecuteStoredProcedureAsync("sp_UserToken_Create", new
                {
                    MaTaiKhoan = user.MaTaiKhoan,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    NgayHetHan = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
                });
                Console.WriteLine("Lưu RefreshToken thành công");

                user.AccessToken = accessToken;
                user.RefreshToken = refreshToken;
                user.ExpiresIn = expiresIn;

                return ApiResponse<TokenResponse>.SuccessResponse(user, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng nhập: {ex.Message}");
                return ApiResponse<TokenResponse>.ErrorResponse($"Lỗi khi đăng nhập: {ex.Message}");
            }
        }

        // Phương thức mới để tạo RefreshToken
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string GenerateJwtToken(TokenResponse user)
        {
            var secret = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                Console.WriteLine("Cấu hình JWT bị thiếu hoặc không hợp lệ");
                throw new ArgumentNullException("Cấu hình JWT (SecretKey, Issuer, hoặc Audience) bị thiếu");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.MaTaiKhoan.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.TenTaiKhoan),
                new Claim(ClaimTypes.Role, user.MaVaiTro.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Tạo JWT token thành công cho MaTaiKhoan: {user.MaTaiKhoan}");
            return tokenString;
        }

        // Phần còn lại của RegisterAsync và ValidateRegisterRequest giữ nguyên
        public async Task<ApiResponse<int>> RegisterAsync(RegisterRequest request)
        {
            if (!ValidateRegisterRequest(request, out string errorMessage))
            {
                Console.WriteLine($"Lỗi xác thực đăng ký: {errorMessage}");
                return ApiResponse<int>.ErrorResponse(errorMessage);
            }

            try
            {
                var hashedPassword = PasswordHasher.HashPassword(request.MatKhau);
                var parameters = new
                {
                    request.TenTaiKhoan,
                    MatKhau = hashedPassword,
                    request.TenHienThi,
                    request.Email,
                    request.Phone,
                    request.MaVaiTro
                };

                Console.WriteLine($"Gọi sp_Register với TenTaiKhoan: {request.TenTaiKhoan}");
                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<dynamic>("sp_Register", parameters);
                if (result != null && result.MaTaiKhoan > 0)
                {
                    Console.WriteLine($"Đăng ký thành công: MaTaiKhoan = {result.MaTaiKhoan}");
                    return ApiResponse<int>.SuccessResponse((int)result.MaTaiKhoan, result.ThongBao ?? "Đăng ký thành công");
                }

                Console.WriteLine("Đăng ký thất bại");
                return ApiResponse<int>.ErrorResponse(result?.ThongBao ?? "Đăng ký thất bại");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng ký: {ex.Message}");
                return ApiResponse<int>.ErrorResponse($"Lỗi khi đăng ký: {ex.Message}");
            }
        }

        private bool ValidateRegisterRequest(RegisterRequest request, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(request.TenTaiKhoan) || request.TenTaiKhoan.Length < 5)
            {
                errorMessage = "Tên tài khoản phải có ít nhất 5 ký tự";
                return false;
            }

            if (string.IsNullOrEmpty(request.MatKhau) || request.MatKhau.Length < 6)
            {
                errorMessage = "Mật khẩu phải có ít nhất 6 ký tự";
                return false;
            }

            if (string.IsNullOrEmpty(request.Email) || !System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                errorMessage = "Email phải có định dạng @gmail.com";
                return false;
            }

            if (string.IsNullOrEmpty(request.Phone) || !System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^\d{10}$"))
            {
                errorMessage = "Số điện thoại phải có 10 chữ số";
                return false;
            }

            return true;
        }
        public async Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    Console.WriteLine("RefreshToken trống");
                    return ApiResponse<TokenResponse>.ErrorResponse("RefreshToken không hợp lệ");
                }

                // Kiểm tra RefreshToken trong UserToken
                var tokenInfo = await _db.QueryFirstOrDefaultStoredProcedureAsync<dynamic>(
                    "sp_UserToken_GetByRefreshToken",
                    new { RefreshToken = request.RefreshToken });

                if (tokenInfo == null || tokenInfo.MaTaiKhoan <= 0)
                {
                    Console.WriteLine("RefreshToken không tồn tại hoặc không hợp lệ");
                    return ApiResponse<TokenResponse>.ErrorResponse("RefreshToken không hợp lệ");
                }

                if (tokenInfo.NgayHetHan < DateTime.UtcNow)
                {
                    Console.WriteLine("RefreshToken đã hết hạn");
                    return ApiResponse<TokenResponse>.ErrorResponse("RefreshToken đã hết hạn");
                }

                // Lấy thông tin tài khoản
                var user = await _db.QueryFirstOrDefaultStoredProcedureAsync<TokenResponse>(
                    "sp_Login_GetById",
                    new { MaTaiKhoan = tokenInfo.MaTaiKhoan });

                if (user == null || user.MaTaiKhoan <= 0)
                {
                    Console.WriteLine("Không tìm thấy tài khoản");
                    return ApiResponse<TokenResponse>.ErrorResponse("Tài khoản không tồn tại");
                }

                // Tạo token mới
                var accessToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();
                var expiresIn = _accessTokenExpiryMinutes * 60;

                // Cập nhật UserToken
                await _db.ExecuteStoredProcedureAsync("sp_UserToken_Create", new
                {
                    MaTaiKhoan = user.MaTaiKhoan,
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken,
                    NgayHetHan = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
                });

                user.AccessToken = accessToken;
                user.RefreshToken = newRefreshToken;
                user.ExpiresIn = expiresIn;

                Console.WriteLine($"Làm mới token thành công cho MaTaiKhoan: {user.MaTaiKhoan}");
                return ApiResponse<TokenResponse>.SuccessResponse(user, "Làm mới token thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi làm mới token: {ex.Message}");
                return ApiResponse<TokenResponse>.ErrorResponse($"Lỗi khi làm mới token: {ex.Message}");
            }
        }
    }

}