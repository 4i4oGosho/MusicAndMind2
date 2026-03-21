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

        public async Task<IActionResult> Index(string searchTerm, string filter)
        {
            var allUsers = _userManager.Users.ToList();
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = adminUsers.Select(a => a.Id).ToHashSet();

            var usersRaw = allUsers
                .Where(u => !adminIds.Contains(u.Id))
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowered = searchTerm.Trim().ToLower();
                usersRaw = usersRaw.Where(u => u.Email != null && u.Email.ToLower().Contains(lowered));
            }

            var users = usersRaw
                .Select(user => new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "Без имейл",
                    EmailConfirmed = user.EmailConfirmed,
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                    OrdersCount = _context.Orders.Count(o => o.UserId == user.Id),
                    TotalSpent = _context.Orders
                        .Where(o => o.UserId == user.Id)
                        .SelectMany(o => o.Items)
                        .Sum(i => (decimal?)i.UnitPrice * i.Quantity) ?? 0,
                    AdminNote = _context.AdminNotes
                        .Where(n => n.UserId == user.Id)
                        .Select(n => n.Note)
                        .FirstOrDefault() ?? ""
                })
                .ToList();

            switch (filter)
            {
                case "locked":
                    users = users.Where(u => u.IsLocked).ToList();
                    break;

                case "unconfirmed":
                    users = users.Where(u => !u.EmailConfirmed).ToList();
                    break;

                case "withorders":
                    users = users.Where(u => u.OrdersCount > 0).ToList();
                    break;
            }

            users = users.OrderBy(u => u.Email).ToList();

            ViewBag.CurrentSearch = searchTerm ?? "";
            ViewBag.CurrentFilter = filter ?? "";

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdminNote(string userId, string note)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction(nameof(Index));

            var existingNote = _context.AdminNotes.FirstOrDefault(n => n.UserId == userId);

            if (existingNote == null)
            {
                _context.AdminNotes.Add(new AdminNote
                {
                    UserId = userId,
                    Note = note ?? ""
                });
            }
            else
            {
                existingNote.Note = note ?? "";
                _context.AdminNotes.Update(existingNote);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction(nameof(Index));

            var relatedNote = _context.AdminNotes.FirstOrDefault(n => n.UserId == id);
            if (relatedNote != null)
            {
                _context.AdminNotes.Remove(relatedNote);
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEmailConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction(nameof(Index));

            user.EmailConfirmed = !user.EmailConfirmed;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLockUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction(nameof(Index));

            var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow;

            if (isLocked)
                user.LockoutEnd = null;
            else
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult UsersOrders()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

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