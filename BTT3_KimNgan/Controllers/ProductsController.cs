using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTT3_KimNgan.Data;
using BTT3_KimNgan.Models;

namespace BTT3_KimNgan.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. TRANG DANH SÁCH SẢN PHẨM (Có tích hợp bộ lọc tiếng Việt)
        // ==========================================================
        public async Task<IActionResult> Index(string category)
        {
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                // So sánh chuỗi tiếng Việt chuẩn xác
                productsQuery = productsQuery.Where(p => p.Category!.Name.Equals(category));
            }

            ViewBag.CurrentCategory = category;
            var products = await productsQuery.ToListAsync();
            return View(products);
        }

        // ==========================================================
        // 2. TRANG XEM CHI TIẾT SẢN PHẨM
        // ==========================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // ==========================================================
        // 3. TRANG THÊM MỚI SẢN PHẨM (Giao diện hiển thị Form)
        // ==========================================================
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // ==========================================================
        // 4. HÀM XỬ LÝ LƯU SẢN PHẨM MỚI (Bấm nút Hoàn tất thêm mới)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ĐÃ CẬP NHẬT: Thêm "ImageUrl" vào chuỗi Bind để mở khóa lưu link ảnh máy tính
        public async Task<IActionResult> Create([Bind("Id,Name,Sku,Price,Description,CategoryId,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ==========================================================
        // 5. TRANG CHỈNH SỬA SẢN PHẨM (Tải dữ liệu cũ lên Form)
        // ==========================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ==========================================================
        // 6. HÀM XỬ LÝ LƯU THÔNG TIN CHỈNH SỬA (Bấm nút Lưu thay đổi)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ĐÃ CẬP NHẬT: Thêm "ImageUrl" vào chuỗi Bind để cho phép sửa đổi link ảnh mới
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Sku,Price,Description,CategoryId,ImageUrl")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ==========================================================
        // 7. TRANG XÁC NHẬN XÓA SẢN PHẨM
        // ==========================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // ==========================================================
        // 8. HÀM XỬ LÝ XÓA VĨNH VIỄN (Bấm nút Xác nhận xóa)
        // ==========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}