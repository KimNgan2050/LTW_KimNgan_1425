using Microsoft.EntityFrameworkCore;
using BTT6_API.Models;       // Giúp nhận diện lớp ApplicationDbContext và Product
using BTT6_API.Repositories; // Giúp nhận diện IProductRepository và ProductRepository

var builder = WebApplication.CreateBuilder(args);

// ====================================================================
// 1. NƠI ĐĂNG KÝ DỊCH VỤ (Tất cả phải nằm TRƯỚC dòng builder.Build())
// ====================================================================

// Đăng ký Cấu hình Kết nối Database SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Repository Pattern (Interface đi kèm với Class hiện thực)
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình CORS (Cho phép kết nối dữ liệu từ Front-end VS Code Live Server)
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowOrigins", policy =>
    {
        policy.WithOrigins(
            "http://127.0.0.1:5500",
            "http://localhost:5500",
            "http://127.0.0.1:5501",
            "http://localhost:5501"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// ====================================================================
// KHỞI TẠO ỨNG DỤNG (Chỉ duy nhất một dòng này làm ranh giới)
// ====================================================================
var app = builder.Build();

// ====================================================================
// 2. CẤU HÌNH HTTP REQUEST PIPELINE (MIDDLEWARE)
// ====================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🎯 ĐÃ CẬP NHẬT: Kích hoạt tính năng đọc file tĩnh (HTML, CSS, JS) trong dự án
app.UseStaticFiles();

app.UseHttpsRedirection();

// Kích hoạt CORS (Bắt buộc phải đứng TRƯỚC UseAuthorization)
app.UseCors("MyAllowOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();