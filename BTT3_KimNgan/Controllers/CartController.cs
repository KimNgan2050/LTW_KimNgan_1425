using Microsoft.AspNetCore.Mvc;
using BTT3_KimNgan.Data;
using BTT3_KimNgan.Models;
using BTT3_KimNgan.Extensions; // Đảm bảo đã gọi để sử dụng GetObjectFromJson và SetObjectAsJson
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BTT3_KimNgan.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Khóa Session dùng chung cho toàn bộ Controller
        private const string CART_SESSION_KEY = "LuxeCart";

        // THAY ĐỔI: Hàm lấy giỏ hàng bây giờ trả về đối tượng ShoppingCart thay vì List<CartItem>
        private ShoppingCart GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(CART_SESSION_KEY);
            if (cart == null)
            {
                cart = new ShoppingCart();
                HttpContext.Session.SetObjectAsJson(CART_SESSION_KEY, cart);
            }
            return cart;
        }

        // ==========================================
        // 1. XỬ LÝ HÀM: THÊM VÀO GIỎ
        // ==========================================
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // THAY ĐỔI: Lấy đối tượng ShoppingCart và gọi hàm AddItem từ class đó
            var cart = GetCart();
            cart.AddItem(product, quantity);

            // Lưu lại đối tượng ShoppingCart vào Session
            HttpContext.Session.SetObjectAsJson(CART_SESSION_KEY, cart);
            return RedirectToAction("Index");
        }

        // ==========================================
        // 2. TRANG HIỂN THỊ GIỎ HÀNG
        // ==========================================
        public IActionResult Index()
        {
            var cart = GetCart();

            // THAY ĐỔI: Tính toán dựa trên thuộc tính cart.Items
            decimal subtotal = cart.Items.Sum(item => item.Product.Price * item.Quantity);
            ViewBag.Subtotal = subtotal;
            ViewBag.Taxes = subtotal * 0.08m; // Thuế 8%
            ViewBag.Total = subtotal + (subtotal * 0.08m);

            // THAY ĐỔI: Vẫn truyền danh sách List<CartItem> (cart.Items) qua View để giao diện cũ của bạn không bị lỗi cấu trúc
            return View(cart.Items);
        }

        // ==========================================
        // 3. XỬ LÝ HÀM: XÓA SẢN PHẨM KHỎI GIỎ
        // ==========================================
        public IActionResult RemoveFromCart(int productId)
        {
            // THAY ĐỔI: Gọi hàm RemoveItem từ class ShoppingCart ngắn gọn hơn rất nhiều
            var cart = GetCart();
            cart.RemoveItem(productId);

            HttpContext.Session.SetObjectAsJson(CART_SESSION_KEY, cart);
            return RedirectToAction("Index");
        }

        // ==========================================
        // 4. XỬ LÝ ĐẶT HÀNG (CHECKOUT) - GET
        // ==========================================
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();

            // THAY ĐỔI: Kiểm tra danh sách sản phẩm bên trong thuộc tính Items
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }

            return View(new Order());
        }

        // ==========================================
        // 5. XỬ LÝ LƯU ĐƠN HÀNG XUỐNG DB - POST
        // ==========================================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // THAY ĐỔI: Tính tổng tiền từ cart.Items
            decimal subtotal = cart.Items.Sum(item => item.Product.Price * item.Quantity);
            decimal totalWithTax = subtotal + (subtotal * 0.08m);

            order.UserId = user.Id;
            order.OrderDate = DateTime.Now;
            order.TotalPrice = totalWithTax;

            // THAY ĐỔI: Duyệt qua danh sách cart.Items để map sang bảng chi tiết đơn hàng
            order.OrderDetails = cart.Items.Select(item => new OrderDetail
            {
                ProductId = item.Product.Id,
                Quantity = item.Quantity,
                Price = item.Product.Price
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Xóa sạch giỏ hàng trong Session sau khi đặt thành công
            HttpContext.Session.Remove(CART_SESSION_KEY);

            return View("OrderCompleted", order.Id);
        }
        // ==========================================
        // 6. API: CẬP NHẬT SỐ LƯỢNG SẢN PHẨM (Dùng cho AJAX)
        // ==========================================
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(p => p.Product.Id == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.RemoveItem(productId); // Nếu giảm về 0 thì xóa luôn khỏi giỏ
                }
                else
                {
                    item.Quantity = quantity; // Cập nhật số lượng mới
                }
                HttpContext.Session.SetObjectAsJson(CART_SESSION_KEY, cart);
            }

            // Tính toán lại toàn bộ các cổng tiền để trả về cho JavaScript cập nhật giao diện
            decimal subtotal = cart.Items.Sum(i => i.Product.Price * i.Quantity);
            decimal taxes = subtotal * 0.08m;
            decimal total = subtotal + taxes;

            // Trả về dữ liệu dạng JSON
            return Json(new
            {
                success = true,
                itemCount = cart.Items.Count,
                itemSubtotal = item != null ? (item.Product.Price * item.Quantity).ToString("#,##0") + " đ" : "0 đ",
                subtotal = subtotal.ToString("#,##0") + " đ",
                taxes = taxes.ToString("#,##0") + " đ",
                total = total.ToString("#,##0") + " đ"
            });
        }
    }
}