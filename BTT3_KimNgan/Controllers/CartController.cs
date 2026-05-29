using Microsoft.AspNetCore.Mvc;
using BTT3_KimNgan.Data;
using BTT3_KimNgan.Models;
using System.Text.Json;

namespace BTT3_KimNgan.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Khóa dùng để lưu trữ dữ liệu giỏ hàng trong Session
        private const string CART_SESSION_KEY = "LuxeCart";

        // Hàm lấy giỏ hàng hiện tại từ Session ra
        private List<CartItem> GetCartItems()
        {
            var sessionData = HttpContext.Session.GetString(CART_SESSION_KEY);
            return sessionData == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(sessionData)!;
        }

        // Hàm lưu giỏ hàng ngược lại vào Session
        private void SaveCartItems(List<CartItem> cart)
        {
            var sessionData = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CART_SESSION_KEY, sessionData);
        }

        // ==========================================
        // 1. XỬ LÝ HÀM: BẤM NÚT THÊM VÀO GIỎ
        // ==========================================
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.Product.Id == id);

            if (cartItem == null)
            {
                cart.Add(new CartItem { Product = product, Quantity = quantity });
            }
            else
            {
                cartItem.Quantity += quantity; // Nếu sản phẩm đã có thì tăng số lượng
            }

            SaveCartItems(cart);
            return RedirectToAction("Index"); // Thêm xong chuyển hướng sang trang Giỏ hàng để xem
        }

        // ==========================================
        // 2. TRANG HIỂN THỊ GIỎ HÀNG (Giống ảnh 4 ban đầu)
        // ==========================================
        public IActionResult Index()
        {
            var cart = GetCartItems();

            // Tính tổng tiền tạm tính
            decimal subtotal = cart.Sum(item => item.Product.Price * item.Quantity);
            ViewBag.Subtotal = subtotal;
            ViewBag.Taxes = subtotal * 0.08m; // Thuế ước tính 8%
            ViewBag.Total = subtotal + (subtotal * 0.08m);

            return View(cart);
        }
    }
}