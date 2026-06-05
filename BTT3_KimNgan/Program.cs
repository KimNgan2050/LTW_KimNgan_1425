using BTT3_KimNgan.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
// Bổ sung thư viện Identity để cấu hình dịch vụ tài khoản và phân quyền
using Microsoft.AspNetCore.Identity;
// Thêm namespace Models để sử dụng lớp ApplicationUser tùy chỉnh
using BTT3_KimNgan.Models;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. ĐĂNG KÝ CÁC DỊCH VỤ (SERVICES)
// ==========================================================
builder.Services.AddControllersWithViews();

// Đăng ký kết nối cơ sở dữ liệu DbContext với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ĐĂNG KÝ DỊCH VỤ RAZOR PAGES (Sửa lỗi crash 'Unable to find the required services')
builder.Services.AddRazorPages();

// Đăng ký dịch vụ ASP.NET Core Identity (Quản lý User & Role dựa trên ApplicationUser mới)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Tùy chọn cấu hình độ bảo mật mật khẩu (Tùy chỉnh theo ý bạn)
    options.Password.RequireDigit = false;            // Không bắt buộc chứa số
    options.Password.RequiredLength = 6;              // Độ dài tối thiểu 6 ký tự
    options.Password.RequireNonAlphanumeric = false;  // Không bắt buộc ký tự đặc biệt
    options.Password.RequireUppercase = false;        // Không bắt buộc chữ in hoa
    options.Password.RequireLowercase = false;        // Không bắt buộc chữ thường
})
.AddDefaultTokenProviders()
.AddDefaultUI() // KHẮC PHỤC: Thêm UI mặc định để Identity chạy được các file Scaffold tiếng Việt của bạn
.AddEntityFrameworkStores<ApplicationDbContext>();

// Cấu hình đường dẫn trang đăng nhập/đăng xuất/từ chối mặc định khi dùng Identity
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Cấu hình dịch vụ Session (Dùng để lưu trữ trạng thái giỏ hàng)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);   // Tự động hủy sau 30 phút không hoạt động
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build(); // <--- CHÚC MỪNG: Hệ thống sẽ vượt qua dòng này mượt mà không còn bị crash!

// ==========================================================
// 2. CẤU HÌNH PIPELINE XỬ LÝ (MIDDLEWARE)
// ==========================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// LƯU Ý THỨ TỰ BẮT BUỘC: Session -> Authentication -> Authorization
app.UseSession();        // 1. Kích hoạt Session cho giỏ hàng
app.UseAuthentication(); // 2. Xác thực danh tính người dùng (Ai đang truy cập?)
app.UseAuthorization();  // 3. Kiểm tra quyền truy cập (Họ được làm gì?)

// Định tuyến cho các trang Đăng ký/Đăng nhập của Identity
app.MapRazorPages();

// ==========================================================
// CẬP NHẬT: ĐỊNH TUYẾN ĐƯỜNG DẪN CHO CÁC KHU VỰC (AREAS)
// ==========================================================
// Tuyến đường areas phải luôn nằm trên tuyến đường default
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Định tuyến đường dẫn mặc định của trang web
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ==========================================================
// 3. THỰC THI NẠP DỮ LIỆU MẪU (SEED DATA TIẾNG VIỆT)
// ==========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Xóa sạch danh mục và sản phẩm cũ nếu phát hiện dữ liệu mẫu tiếng Anh cũ (tránh xung đột)
        if (context.Categories.Any(c => c.Name == "Outerwear"))
        {
            context.Products.RemoveRange(context.Products);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();
        }

        // Tự động khởi tạo danh mục TIẾNG VIỆT mới nếu bảng Danh mục trống
        if (!context.Categories.Any())
        {
            var aoKhoac = new BTT3_KimNgan.Models.Category { Name = "Áo khoác" };
            var vayDam = new BTT3_KimNgan.Models.Category { Name = "Váy đầm" };
            var phuKien = new BTT3_KimNgan.Models.Category { Name = "Phụ kiện" };
            var tShirt = new BTT3_KimNgan.Models.Category { Name = "T-shirt" };
            var hoodie = new BTT3_KimNgan.Models.Category { Name = "Hoodie" };

            context.Categories.AddRange(aoKhoac, vayDam, phuKien, tShirt, hoodie);
            context.SaveChanges();

            // Tự động nạp sản phẩm mẫu gắn liền với danh mục vừa tạo
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