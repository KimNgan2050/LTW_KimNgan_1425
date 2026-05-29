using Microsoft.EntityFrameworkCore;
using BTT3_KimNgan.Models;

namespace BTT3_KimNgan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng vào Database
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Cấu hình mối quan hệ giữa các bảng (Giúp Database tạo khóa ngoại chuẩn xác)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                .OnDelete(DeleteBehavior.Cascade); // Xóa sản phẩm thì tự động xóa biến thể

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Size)
                .WithMany(s => s.ProductVariants)
                .HasForeignKey(pv => pv.SizeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}