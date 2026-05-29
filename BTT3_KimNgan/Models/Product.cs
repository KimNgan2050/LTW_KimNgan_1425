namespace BTT3_KimNgan.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        // Quan hệ dữ liệu
        public Category? Category { get; set; }
        public List<ProductImage> Images { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();

        public string ImageUrl { get; set; } = "https://via.placeholder.com/600x800"; // Mặc định nếu không nhập ảnh
    }
}
