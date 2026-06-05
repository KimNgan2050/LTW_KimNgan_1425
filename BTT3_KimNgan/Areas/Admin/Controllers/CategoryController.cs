using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BTT3_KimNgan.Models;
using BTT3_KimNgan.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BTT3_KimNgan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // 1. TRANG DANH SÁCH DANH MỤC (INDEX)
        public IActionResult Index()
        {
            var categories = _context.Categories.Include(c => c.Products).ToList();
            return View(categories);
        }

        // 2. THÊM DANH MỤC (CREATE - GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // THÊM DANH MỤC (CREATE - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 3. CHỈNH SỬA DANH MỤC (EDIT - GET)
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // CHỈNH SỬA DANH MỤC (EDIT - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 4. XÓA DANH MỤC (DELETE - GET: CẢNH BÁO)
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = _context.Categories.Include(c => c.Products).FirstOrDefault(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // XÓA DANH MỤC (DELETE - POST: CHÍNH THỨC XÓA)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.Categories.Include(c => c.Products).FirstOrDefault(m => m.Id == id);
            if (category != null)
            {
                // Khi xóa Category, xóa các tệp ảnh sản phẩm bị xóa cascade trên disk
                foreach (var product in category.Products)
                {
                    if (!string.IsNullOrEmpty(product.ImageUrl) && !product.ImageUrl.Contains("placeholder") && !product.ImageUrl.StartsWith("http"))
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        var imagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
