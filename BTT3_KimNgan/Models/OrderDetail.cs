using System.ComponentModel.DataAnnotations.Schema;

namespace BTT3_KimNgan.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Bổ sung Data Annotation cấu hình kiểu dữ liệu decimal cho SQL Server
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}