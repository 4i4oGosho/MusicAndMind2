using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAndMind2.Data;
using MusicAndMind2.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile ImageFile, IFormFile SoundFile)
        {
            if (!ModelState.IsValid)
                return View(product);

            // ===== IMAGE =====
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "shop");
                Directory.CreateDirectory(imagesDir);

                var imgExt = Path.GetExtension(ImageFile.FileName);
                var imgName = $"{Guid.NewGuid()}{imgExt}";
                var imgPath = Path.Combine(imagesDir, imgName);

                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "/images/shop/" + imgName;
            }

            // ===== SOUND (DEMO) =====
            if (SoundFile != null && SoundFile.Length > 0)
            {
                var audioDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audio", "demo");
                Directory.CreateDirectory(audioDir);

                var sndExt = Path.GetExtension(SoundFile.FileName);
                var sndName = $"{Guid.NewGuid()}{sndExt}";
                var sndPath = Path.Combine(audioDir, sndName);

                using (var stream = new FileStream(sndPath, FileMode.Create))
                {
                    await SoundFile.CopyToAsync(stream);
                }

                product.SoundUrl = "/audio/demo/" + sndName;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // След запис -> списъка с продукти
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
        public IActionResult EditProduct(Product model)
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