using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Data;

namespace MusicAndMind2.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string searchTerm, string category, string sortOrder)
        {
            var query = _context.Products
                .Where(p => p.IsAvailable)
                .AsQueryable();

            // Търсене
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var loweredSearch = searchTerm.ToLower();

                query = query.Where(p =>
                    p.Name.ToLower().Contains(loweredSearch) ||
                    (p.Description != null && p.Description.ToLower().Contains(loweredSearch)) ||
                    (p.LongDescription != null && p.LongDescription.ToLower().Contains(loweredSearch)) ||
                    (p.Category != null && p.Category.ToLower().Contains(loweredSearch)) ||
                    (p.FrequencyInfo != null && p.FrequencyInfo.ToLower().Contains(loweredSearch))
                );
            }

            // Филтър по категория
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category == category);
            }

            // Сортиране
            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;

                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;

                case "name_asc":
                    query = query.OrderBy(p => p.Name);
                    break;

                case "oldest":
                    query = query.OrderBy(p => p.CreatedAt);
                    break;

                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            var products = query.ToList();

            var categories = _context.Products
                .Where(p => p.IsAvailable && !string.IsNullOrWhiteSpace(p.Category))
                .Select(p => p.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ViewBag.Categories = categories;
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSort = sortOrder;

            return View(products);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.Id == id && p.IsAvailable);

            if (product == null)
                return NotFound();

            return View(product);
        }

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