using System.ComponentModel.DataAnnotations.Schema; // Thêm dòng này ở đầu file

namespace BTT6_API.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [Column(TypeName = "decimal(18,2)")] // Thêm dòng cấu hình này ở đây
        public decimal Price { get; set; }

        public string? Description { get; set; }
    }
}