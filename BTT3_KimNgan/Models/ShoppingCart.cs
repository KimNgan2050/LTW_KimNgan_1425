using System.Collections.Generic;
using System.Linq;

namespace BTT3_KimNgan.Models
{
    public class ShoppingCart
    {
        // Danh sách các món hàng bên trong giỏ
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // Hàm xử lý thêm sản phẩm vào giỏ
        public void AddItem(Product product, int quantity)
        {
            var item = Items.FirstOrDefault(p => p.Product.Id == product.Id);
            if (item == null)
            {
                Items.Add(new CartItem { Product = product, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity; // Nếu đã có thì tăng số lượng
            }
        }

        // Hàm xử lý xóa sản phẩm khỏi giỏ (Phục vụ nút Xóa)
        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(p => p.Product.Id == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        // Hàm xóa sạch giỏ hàng sau khi đã đặt hàng thành công
        public void Clear()
        {
            Items.Clear();
        }
    }
}