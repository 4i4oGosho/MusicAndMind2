using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Models;

namespace MusicAndMind2.Controllers
{
    public class ShopController : Controller
    {
        // 🛒 Примерни продукти
        public static readonly List<Product> Products = new()

        {
            new Product
            {   
                Id = 1,
                Name = "Тибетска купа (432 Hz)",
                Description = "Кристална купа, чийто звук насърчава дълбока релаксация и вътрешен баланс.",
                Price = 49.99m,
                ImageUrl = "/images/shop/bowl1.jpg",
                SoundUrl = "/audio/bowl1.mp3",
                FrequencyInfo = "Подпомага алфа мозъчни вълни (8–12 Hz).",
                Category = "Купи"
            },
            new Product
            {
                Id = 2,
                Name = "Камертон (528 Hz)",
                Description = "Този камертон се използва за енергийно пречистване и е известен като 'честотата на любовта'.",
                Price = 29.99m,
                ImageUrl = "/images/shop/fork1.jpg",
                SoundUrl = "/audio/fork1.mp3",
                FrequencyInfo = "528 Hz се асоциира с усещане за мир и вътрешна хармония.",
                Category = "Камертон"
            },
            new Product
            {
                Id = 3,
                Name = "Гонг – Златен резонанс",
                Description = "Малък гонг с богат, вибрационен тон, подходящ за дълбоки медитации.",
                Price = 79.99m,
                ImageUrl = "/images/shop/gong1.jpg",
                SoundUrl = "/audio/gong1.mp3",
                FrequencyInfo = "Тета честоти (4–7 Hz), подходящи за сън и релакс.",
                Category = "Гонгове"
            },
            new Product
            {
                Id = 4,
                Name = "Калимба – Звуков поток",
                Description = "Инструмент с топъл, дървен тон, който стимулира креативността.",
                Price = 39.99m,
                ImageUrl = "/images/shop/kalimba1.jpg",
                SoundUrl = "/audio/kalimba1.mp3",
                FrequencyInfo = "Стимулира бета вълни (13–30 Hz) – фокус и вдъхновение.",
                Category = "Калимби"
            }
        };

        // 🪷 Магазин
        [HttpGet]
        public IActionResult Test() => View();
        public IActionResult Index()
        {
            return View(Products);
        }

        // 🛍️ При натискане на "Купи" – максимално опростено
        [HttpPost]
        [IgnoreAntiforgeryToken]  // махаме проверката за токен, за да не пречи
        public IActionResult Checkout(int productId, string paymentMethod)
        {
            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(paymentMethod))
                paymentMethod = "card";

            var paymentText = paymentMethod == "cod" ? "Наложен платеж" : "Карта";

            TempData["ProductName"] = product.Name;
            TempData["Price"] = product.Price.ToString("0.00");
            TempData["Payment"] = paymentText;

            // за момента може да върнем и чист текст, за да е супер явно
            // return Content($"OK: {product.Name} / {paymentText} / {product.Price} лв.");

            return RedirectToAction(nameof(Success));
        }

        // 💫 Страница "Благодарим за поръчката"
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
