namespace BTT3_KimNgan.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Stock { get; set; } // Số lượng tồn kho của size này

        // Quan hệ dữ liệu
        public Product? Product { get; set; }
        public Size? Size { get; set; }
    }
}
