using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAndMind2.Data;
using MusicAndMind2.Models;

namespace MusicAndMind2.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🛒 Магазин – показва САМО наличните продукти
        [HttpGet]
        public IActionResult Index()
        {
            var products = _context.Products
                .Where(p => p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(products);
        }

        // 🔍 Детайли за продукт
        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.Id == id && p.IsAvailable);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // 💳 Поръчка (визуална)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Checkout(int productId, string paymentMethod)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.Id == productId && p.IsAvailable);

            if (product == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(paymentMethod))
                paymentMethod = "card";

            var paymentText = paymentMethod == "cod"
                ? "Наложен платеж"
                : "Карта";

            TempData["ProductName"] = product.Name;
            TempData["Price"] = product.Price.ToString("0.00");
            TempData["Payment"] = paymentText;

            return RedirectToAction(nameof(Success));
        }

        // ✅ Успешна покупка
        [HttpGet]
        public IActionResult Success()
        {
            if (TempData["ProductName"] == null)
                return RedirectToAction(nameof(Index));

            ViewBag.ProductName = TempData["ProductName"];
            ViewBag.Price = TempData["Price"];
            ViewBag.Payment = TempData["Payment"];

            return View();
        }
    }
}
