using BTT3_KimNgan.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// 1. ĐĂNG KÝ CÁC DỊCH VỤ (SERVICES)
builder.Services.AddControllersWithViews();

// Đăng ký bộ kết nối cơ sở dữ liệu DbContext với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình dịch vụ bộ nhớ đệm Session (Dùng cho tính năng giỏ hàng)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Giỏ hàng tự hủy sau 30 phút không hoạt động

    // ĐÃ SỬA: Bổ sung thuộc tính .Cookie vào đây để hết lỗi gạch đỏ
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 2. CẤU HÌNH PIPELINE XỬ LÝ (MIDDLEWARE)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kích hoạt tính năng Session (Bắt buộc đứng trước UseAuthorization)
app.UseSession();

app.UseAuthorization();

// Định tuyến đường dẫn mặc định khi mở trang web
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ==========================================================
// THỰC THI NẠP DỮ LIỆU MẪU (SEED DATA) - PHIÊN BẢN AN TOÀN TUYỆT ĐỐI
// ==========================================================
// ==========================================================
// THỰC THI NẠP DỮ LIỆU MẪU (SEED DATA) - PHIÊN BẢN TIẾNG VIỆT MỚI 100%
// ==========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Xóa sạch danh mục và sản phẩm cũ để tránh xung đột tiếng Anh - tiếng Việt
        // Lưu ý: Chỉ nên chạy dòng xóa này 1 lần duy nhất để làm sạch DB cũ
        if (context.Categories.Any(c => c.Name == "Outerwear"))
        {
            context.Products.RemoveRange(context.Products);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();
        }

        // Kiểm tra và tự động tạo danh mục TIẾNG VIỆT nếu bảng trống
        if (!context.Categories.Any())
        {
            var aoKhoac = new BTT3_KimNgan.Models.Category { Name = "Áo khoác" };
            var vayDam = new BTT3_KimNgan.Models.Category { Name = "Váy đầm" };
            var phuKien = new BTT3_KimNgan.Models.Category { Name = "Phụ kiện" };
            var tShirt = new BTT3_KimNgan.Models.Category { Name = "T-shirt" };
            var hoodie = new BTT3_KimNgan.Models.Category { Name = "Hoodie" };

            context.Categories.AddRange(aoKhoac, vayDam, phuKien, tShirt, hoodie);
            context.SaveChanges();

            // Tự động nạp lại sản phẩm mẫu gắn với danh mục mới
            if (!context.Products.Any())
            {
                context.Products.Add(new BTT3_KimNgan.Models.Product
                {
                    Name = "STRUCTURED WOOL OVERCOAT",
                    Sku = "LFA-2024-WO-01",
                    Price = 895000,
                    Description = "Được may thủ công tinh tế từ chất liệu 100% len cao cấp...",
                    CategoryId = aoKhoac.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?q=80&w=600"
                });

                context.Products.Add(new BTT3_KimNgan.Models.Product
                {
                    Name = "ASYMMETRIC SILK MIDI DRESS",
                    Sku = "LFA-2024-SD-02",
                    Price = 450000,
                    Description = "Được thiết kế sang trọng từ 100% lụa dâu tằm tự nhiên...",
                    CategoryId = vayDam.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1595777457583-95e059d581b8?q=80&w=600"
                });

                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[CẢNH BÁO SEED DATA]: Lỗi: {ex.Message}");
    }
}
// Khởi chạy ứng dụng
app.Run();