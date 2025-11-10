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
                Category = "Купи",
                LongDescription = "Тази тибетска купа е изработена ръчно в подножието на Хималаите и създава меки, дълбоки вибрации, които хармонизират тялото и ума. Звукът ѝ подпомага баланса между енергийните центрове, отпуска нервната система и помага за концентрация по време на медитация или йога. Идеална за звукови терапии, релаксация и лична употреба."
            },
            new Product
            {
                Id = 2,
                Name = "Камертон (528 Hz)",
                Description = "Камертон за енергийно пречистване и „честотата на любовта“.",
                Price = 29.99m,
                ImageUrl = "/images/shop/fork1.jpg",
                SoundUrl = "/audio/fork1.mp3",
                FrequencyInfo = "528 Hz се асоциира с усещане за мир и вътрешна хармония.",
                Category = "Камертон",
                LongDescription = "Този камертон е настроен на 528 Hz — честота, често наричана „честотата на любовта“. Тя се използва в звукотерапията за регенерация на клетките и намаляване на стреса. Изключително подходящ за енергийни практики, дихателни сесии и медитации, насочени към сърдечната чакра."
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
                Category = "Гонгове",
                LongDescription = "Златният гонг е изработен от висококачествен бронз и произвежда дълбок, хармоничен тон, който прониква през съзнанието и създава усещане за пространственост и покой. Идеален за медитации, звукотерапия и дълбока релаксация."
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
                Category = "Калимби",
                LongDescription = "Калимбата е африкански музикален инструмент с дървен резонатор и метални езичета, които създават меки, хармонични звуци. Чудесен избор за начинаещи и напреднали, които търсят баланс между музика и медитация."
            }
        };

        // 🪷 Магазин
        [HttpGet]
        public IActionResult Index() => View(Products);

        // 🧭 Детайли за конкретен продукт
        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // 🛍️ При натискане на "Купи"
        [HttpPost]
        [IgnoreAntiforgeryToken]
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
