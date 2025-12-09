using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MusicAndMind2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IConfiguration _config;

        public CartController(IConfiguration config)
        {
            _config = config;
        }

        // 🔐 Отделна кошница за всеки потребител (в сесията)
        private string CartSessionKey =>
            User.Identity!.IsAuthenticated
                ? $"CartItems_{User.Identity.Name}"
                : "CartItems_Guest";

        // 🧠 Взимане на кошница
        private List<Product> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson != null
                ? JsonConvert.DeserializeObject<List<Product>>(cartJson) ?? new List<Product>()
                : new List<Product>();
        }

        // 💾 Запазване на кошницата
        private void SaveCart(List<Product> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        // ➕ Добавяне на продукт (старият работещ вариант)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AddToCart(int id)
        {
            var product = ShopController.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            var cart = GetCart();
            cart.Add(product);
            SaveCart(cart);

            // AJAX добавяне
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, count = cart.Count });

            return RedirectToAction("Index");
        }

        // 🛒 Преглед на кошницата
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.CartCount = cart.Count;
            return View(cart);
        }

        // ❌ Премахване
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.Id == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction(nameof(Index));
        }

        // 🧹 Изчистване
        [HttpPost]
        public IActionResult ClearCart()
        {
            SaveCart(new List<Product>());
            return RedirectToAction(nameof(Index));
        }

        // 🧾 Checkout (страницата с формата)
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(p => p.Price);
            return View();
        }

        // ✅ Потвърждаване на поръчката + изпращане на e-mail
        [HttpPost]
        public IActionResult Checkout(string name, string address, string city, string phone)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            // e-mail на логнатия потребител (при теб username = e-mail)
            string toEmail = User.Identity?.Name ?? "unknown@local";
            var total = cart.Sum(p => p.Price);

            // 📝 Текст на съобщението
            var sb = new StringBuilder();
            sb.AppendLine($"Здравей, {name}!");
            sb.AppendLine();
            sb.AppendLine("Благодарим ти за поръчката в Music & Mind.");
            sb.AppendLine();
            sb.AppendLine("Детайли за поръчката:");

            foreach (var p in cart)
            {
                sb.AppendLine($" • {p.Name} - {p.Price:0.00} €");
            }

            sb.AppendLine();
            sb.AppendLine($"Обща сума: {total:0.00} €");
            sb.AppendLine();
            sb.AppendLine("Данни за доставка:");
            sb.AppendLine($"Име: {name}");
            sb.AppendLine($"Град: {city}");
            sb.AppendLine($"Адрес: {address}");
            sb.AppendLine($"Телефон: {phone}");
            sb.AppendLine();
            sb.AppendLine("Начин на плащане: Наложен платеж при доставка.");
            sb.AppendLine("Очакван срок за доставка: 3–5 работни дни.");
            sb.AppendLine();
            sb.AppendLine("С хармония,");
            sb.AppendLine("Music & Mind");

            // 📧 Изпращане на e-mail
            try
            {
                // четем настройките от appsettings.json -> "SMTP": { ... }
                var host = _config["SMTP:Host"];
                int port = int.TryParse(_config["SMTP:Port"], out var parsedPort) ? parsedPort : 587;
                bool enableSsl = bool.TryParse(_config["SMTP:EnableSSL"], out var parsedSsl) ? parsedSsl : true;
                var user = _config["SMTP:User"];
                var pass = _config["SMTP:Password"];
                var from = _config["SMTP:From"] ?? user;
                var fromName = _config["SMTP:FromName"] ?? "Music & Mind";

                var message = new MailMessage();
                message.From = new MailAddress(from, fromName);
                message.To.Add(toEmail);
                message.Subject = "Потвърждение на поръчка";
                message.Body = sb.ToString();
                message.IsBodyHtml = false;

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.EnableSsl = enableSsl;
                    smtp.Credentials = new NetworkCredential(user, pass);
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // ❗ Записваме грешката, за да я видиш в OrderSuccess
                TempData["MailError"] = ex.ToString();
            }

            // 🗑 Изчистваме кошницата след поръчка
            SaveCart(new List<Product>());

            TempData["OrderName"] = name;
            TempData["OrderPayment"] = "Наложен платеж 🚚";

            return RedirectToAction(nameof(OrderSuccess));
        }

        // 🎉 Успешна поръчка
        public IActionResult OrderSuccess()
        {
            ViewBag.Name = TempData["OrderName"];
            ViewBag.Payment = TempData["OrderPayment"];
            ViewBag.MailError = TempData["MailError"]; // добавяме грешката, ако има
            return View();
        }
    }
}
