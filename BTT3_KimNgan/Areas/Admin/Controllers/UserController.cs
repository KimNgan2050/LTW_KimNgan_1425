using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BTT3_KimNgan.Models;
using BTT3_KimNgan.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BTT3_KimNgan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. TRANG DANH SÁCH TÀI KHOẢN (INDEX)
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRolesList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesList.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    Address = user.Address,
                    Age = user.Age,
                    Role = roles.FirstOrDefault() ?? "None"
                });
            }

            return View(userRolesList);
        }

        // 2. CHỈNH SỬA TÀI KHOẢN & PHÂN QUYỀN (EDIT - GET)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Address = user.Address,
                Age = user.Age,
                Role = roles.FirstOrDefault() ?? "None"
            };

            // Tạo danh sách Roles cho Dropdown
            ViewBag.RoleList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Admin", Value = SD.Role_Admin },
                new SelectListItem { Text = "Employee", Value = SD.Role_Employee },
                new SelectListItem { Text = "Customer", Value = SD.Role_Customer },
                new SelectListItem { Text = "Company", Value = SD.Role_Company }
            };

            return View(model);
        }

        // CHỈNH SỬA TÀI KHOẢN & PHÂN QUYỀN (EDIT - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                // Cập nhật thông tin cơ bản
                user.FullName = model.FullName;
                user.Address = model.Address;
                user.Age = model.Age;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Cập nhật vai trò (Role)
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var newRole = model.Role;

                    if (!currentRoles.Contains(newRole))
                    {
                        // Xóa các vai trò cũ
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        // Thêm vai trò mới
                        if (newRole != "None")
                        {
                            await _userManager.AddToRoleAsync(user, newRole);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.RoleList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Admin", Value = SD.Role_Admin },
                new SelectListItem { Text = "Employee", Value = SD.Role_Employee },
                new SelectListItem { Text = "Customer", Value = SD.Role_Customer },
                new SelectListItem { Text = "Company", Value = SD.Role_Company }
            };

            return View(model);
        }

        // 3. XÓA TÀI KHOẢN (DELETE - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Ngăn tự xóa chính mình
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && currentUser.Id == user.Id)
                {
                    TempData["Error"] = "Bạn không thể tự xóa tài khoản của chính mình!";
                    return RedirectToAction(nameof(Index));
                }

                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
