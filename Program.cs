using System.Data;
using HotelManagement.DataReader;
using HotelManagement.Services;
using HotelManagement_BackEnd.Application.Interfaces;
using HotelManagement_BackEnd.Application.Services;
using HotelManagement_BackEnd.Helpers;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký DatabaseDapper
builder.Services.AddScoped<IDbConnection>(sql => new SqlConnection(builder.Configuration.GetConnectionString("Dbcontext")));
builder.Services.AddSingleton<DatabaseDapper>();

// Đăng ký các services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IServiceTypeService, ServiceTypeService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IBookingServiceService, BookingServiceService>();
// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // URL của React frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp"); // Đảm bảo API sử dụng đúng policy CORS
app.UseAuthorization();
app.MapControllers();
app.Run();
