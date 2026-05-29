namespace BTT3_KimNgan.Models
{
    public class Size
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // XS, S, M, L, XL

        // Quan hệ: Một kích thước có thể áp dụng cho nhiều biến thể sản phẩm
        public List<ProductVariant> ProductVariants { get; set; } = new();
    }
}
