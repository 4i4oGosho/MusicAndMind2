using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Data;
using MusicAndMind2.Models;

namespace MusicAndMind2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // === УПРАВЛЕНИЕ НА ПОТРЕБИТЕЛИ =====================

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // не трие нищо друго – само Identity записа
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // върни списъка с потребители + грешки
                var users = _userManager.Users.ToList();
                return View("Index", users);
            }

            return RedirectToAction(nameof(Index));
        }

        // === УПРАВЛЕНИЕ НА ПРОДУКТИ =========================

        // Списък с продукти
        public IActionResult ProductList()
        {
            var products = _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(products);
        }

        // Създаване на продукт - GET
        public IActionResult CreateProduct()
        {
            var model = new Product();
            return View(model);
        }

        // Създаване на продукт - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(Product model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CreatedAt = DateTime.Now;

            _context.Products.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(ProductList));
        }

        // Редакция - GET
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Редакция - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Products.Update(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(ProductList));
        }

        // Изтриване - GET (екран за потвърждение на продукт)
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Изтриване - POST (истинското триене на продукт)
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProductConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(ProductList));
        }
    }
}
