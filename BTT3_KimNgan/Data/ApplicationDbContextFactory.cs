using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using BTT3_KimNgan.Data;

namespace BTT3_KimNgan.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Hardcode tạm thời chuỗi kết nối tại đây để EF Core tìm thấy ngay lập tức khi Migration
            string connectionString = "Server=.\\SQLEXPRESS;Database=LuxeFashionDb;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}