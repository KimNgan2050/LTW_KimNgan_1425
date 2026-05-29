namespace BTT3_KimNgan.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; } // Số lượng mua
    }
}