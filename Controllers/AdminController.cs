using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Data;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                var users = _userManager.Users.ToList();
                return View("Index", users);
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ Поръчки по потребители
        public IActionResult UsersOrders()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // ✅ Поръчки на конкретен потребител
        public IActionResult UserOrders(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            var orders = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == id)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            ViewBag.UserEmail = user.Email;
            ViewBag.UserId = user.Id;

            return View(orders);
        }

        public IActionResult ProductList()
        {
            var products = _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(products);
        }

        public IActionResult CreateProduct()
        {
            return View(new MusicAndMind2.Models.Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(MusicAndMind2.Models.Product model)
        {
            if (!ModelState.IsValid) return View(model);

            model.CreatedAt = DateTime.Now;

            _context.Products.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(ProductList));
        }

        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(MusicAndMind2.Models.Product model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Products.Update(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(ProductList));
        }

        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProductConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(ProductList));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleAvailability(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            product.IsAvailable = !product.IsAvailable;
            _context.SaveChanges();

            return RedirectToAction(nameof(ProductList));
        }

    }
}
