using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Bắt buộc phải có để sử dụng SelectListItem cho Dropdown
using BTT3_KimNgan.Models;
using BTT3_KimNgan.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BTT3_KimNgan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Khóa bảo mật cấp cao: Chỉ tài khoản Admin chỉnh trong SQL mới vào được
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. TRANG DANH SÁCH SẢN PHẨM (INDEX)
        // ==========================================================
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // ==========================================================
        // 2. TRANG XEM CHI TIẾT SẢN PHẨM (DETAILS)
        // ==========================================================
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var product = _context.Products.FirstOrDefault(m => m.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // ==========================================================
        // 3. TRANG THÊM SẢN PHẨM (CREATE - GET: HIỂN THỊ FORM)
        // ==========================================================
        [HttpGet]
        public IActionResult Create()
        {
            // Ép hệ thống nạp trực tiếp danh sách danh mục động bằng .ToList()
            ViewBag.CategoryList = _context.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,          // Hiển thị tên chữ (Áo khoác, Váy đầm...)
                Value = i.Id.ToString() // Giá trị số Id ẩn ngầm (1, 2, 3...)
            }).ToList();

            return View();
        }

        // TRANG THÊM SẢN PHẨM (CREATE - POST: XỬ LÝ LƯU VÀO SQL)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Phòng hờ: Nếu Admin điền thiếu thông tin, nạp lại dữ liệu động để Dropdown không bị lỗi trắng trang
            ViewBag.CategoryList = _context.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();

            return View(product);
        }

        // ==========================================================
        // 4. TRANG SỬA SẢN PHẨM (EDIT - GET: ĐỔ DỮ LIỆU CŨ LÊN FORM)
        // ==========================================================
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            // Cập nhật: Đổ danh sách danh mục động cho trang chỉnh sửa sản phẩm
            ViewBag.CategoryList = _context.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();

            return View(product);
        }

        // TRANG SỬA SẢN PHẨM (EDIT - POST: CẬP NHẬT BIẾN ĐỘNG VÀO SQL)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Products.Update(product);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Phòng hờ: Nếu sửa lỗi form, nạp lại danh mục tránh crash trang Dropdown
            ViewBag.CategoryList = _context.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();

            return View(product);
        }

        // ==========================================================
        // 5. TRANG XÓA SẢN PHẨM (DELETE - GET: FORM CẢNH BÁO)
        // ==========================================================
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = _context.Products.FirstOrDefault(m => m.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // TRANG XÓA SẢN PHẨM (DELETE - POST: CHÍNH THỨC XÓA KHỎI SQL)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}