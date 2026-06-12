using Microsoft.EntityFrameworkCore;
// Bổ sung thư viện Identity để có thể kế thừa IdentityDbContext
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BTT3_KimNgan.Models;

namespace BTT3_KimNgan.Data
{
    // CẬP NHẬT: Thay đổi từ IdentityDbContext thành IdentityDbContext<ApplicationUser> 
    // để hệ thống hiểu và thêm cột FullName vào bảng cơ sở dữ liệu AspNetUsers
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng cơ sở dữ liệu của ứng dụng
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        // Cấu hình mối quan hệ giữa các bảng (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BẮT BUỘC: Giữ nguyên dòng base này để Identity tự động cấu hình các bảng mặc định của nó
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình giá tiền (Price) thuộc kiểu decimal(18,2) để tránh lỗi cảnh báo khi Migration
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // 2. Cấu hình bảng trung gian ProductVariant (Kết hợp giữa Product và Size)
            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany() // Nếu trong lớp Product không khai báo List<ProductVariant> thì để trống
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa sản phẩm thì tự động xóa các biến thể của nó

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Size)
                .WithMany(s => s.ProductVariants)
                .HasForeignKey(pv => pv.SizeId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa size thì tự động xóa biến thể liên quan

            // 3. Cấu hình xóa Cascade giữa Category và Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}