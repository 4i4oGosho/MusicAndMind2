using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using MusicAndMind2.Data;
using System.Security.Claims;

namespace MusicAndMind2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public CartController(IConfiguration config, ApplicationDbContext db)
        {
            _config = config;
            _db = db;
        }

        private string CartSessionKey =>
            User.Identity!.IsAuthenticated
                ? $"CartItems_{User.Identity.Name}"
                : "CartItems_Guest";

        private List<CartSessionItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson != null
                ? JsonConvert.DeserializeObject<List<CartSessionItem>>(cartJson) ?? new List<CartSessionItem>()
                : new List<CartSessionItem>();
        }

        private void SaveCart(List<CartSessionItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        // ✅ Взимаме продукта от БД (а не от ShopController.Products)
        private Product? GetProductFromDb(int id)
        {
            return _db.Products.FirstOrDefault(p => p.Id == id && p.IsAvailable);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AddToCart(int id)
        {
            var product = GetProductFromDb(id);
            if (product == null) return NotFound();

            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.Product.Id == id);

            if (existing != null) existing.Quantity++;
            else cart.Add(new CartSessionItem { Product = product, Quantity = 1 });

            SaveCart(cart);

            var totalCount = cart.Sum(x => x.Quantity);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, count = totalCount });

            return RedirectToAction("Index");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AddToCartQty(int id, int qty)
        {
            if (qty < 1) qty = 1;

            var product = GetProductFromDb(id);
            if (product == null) return NotFound();

            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.Product.Id == id);

            if (existing != null) existing.Quantity += qty;
            else cart.Add(new CartSessionItem { Product = product, Quantity = qty });

            SaveCart(cart);

            var totalCount = cart.Sum(x => x.Quantity);

            return Json(new { success = true, count = totalCount });
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.CartCount = cart.Sum(x => x.Quantity);
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.Id == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            SaveCart(new List<CartSessionItem>());
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            // Сумирай цената на всички артикули в количката
            var total = cart.Sum(i => i.Product.Price * i.Quantity);

            // Предавай количката и общата стойност в view-то
            ViewBag.Cart = cart;
            ViewBag.Total = total;

            return View();
        }

        [HttpPost]
        public IActionResult Checkout(string name, string address, string city, string phone)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var customerEmail = User.Identity?.Name ?? "unknown@local";
            var total = cart.Sum(i => i.Product.Price * i.Quantity);

            // Запис в DB
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    UserEmail = customerEmail,
                    Name = name,
                    City = city,
                    Address = address,
                    Phone = phone,
                    TotalAmount = total,
                    CreatedAt = DateTime.Now
                };

                foreach (var item in cart)
                {
                    order.Items.Add(new OrderItem
                    {
                        ProductId = item.Product.Id,
                        ProductName = item.Product.Name,
                        UnitPrice = item.Product.Price,
                        Quantity = item.Quantity
                    });
                }

                _db.Orders.Add(order);
                _db.SaveChanges();
            }
            catch { }

            // Email клиент
            var customerSb = new StringBuilder();
            customerSb.AppendLine($"Здравей, {name}!");
            customerSb.AppendLine();
            customerSb.AppendLine("Благодарим ти за поръчката в Music & Mind.");
            customerSb.AppendLine();
            customerSb.AppendLine("Детайли за поръчката:");

            foreach (var item in cart)
                customerSb.AppendLine($" • {item.Product.Name} — {item.Quantity} x {item.Product.Price:0.00} €");

            customerSb.AppendLine();
            customerSb.AppendLine($"Обща сума: {total:0.00} €");
            customerSb.AppendLine();
            customerSb.AppendLine("Данни за доставка:");
            customerSb.AppendLine($"Име: {name}");
            customerSb.AppendLine($"Град: {city}");
            customerSb.AppendLine($"Адрес: {address}");
            customerSb.AppendLine($"Телефон: {phone}");
            customerSb.AppendLine();
            customerSb.AppendLine("Начин на плащане: Наложен платеж при доставка.");
            customerSb.AppendLine("Очакван срок за доставка: 3–5 работни дни.");
            customerSb.AppendLine();
            customerSb.AppendLine("С хармония,");
            customerSb.AppendLine("Music & Mind");

            // Email админ
            var adminSb = new StringBuilder();
            adminSb.AppendLine("Нова поръчка в Music & Mind");
            adminSb.AppendLine();
            adminSb.AppendLine("Детайли:");

            foreach (var item in cart)
                adminSb.AppendLine($" • {item.Product.Name} — {item.Quantity} x {item.Product.Price:0.00} €");

            adminSb.AppendLine();
            adminSb.AppendLine($"Обща сума: {total:0.00} €");
            adminSb.AppendLine();
            adminSb.AppendLine("Доставка:");
            adminSb.AppendLine($"Име: {name}");
            adminSb.AppendLine($"Град: {city}");
            adminSb.AppendLine($"Адрес: {address}");
            adminSb.AppendLine($"Телефон: {phone}");
            adminSb.AppendLine();
            adminSb.AppendLine($"Клиент (акаунт email): {customerEmail}");

            try
            {
                var host = _config["SMTP:Host"];
                int port = int.TryParse(_config["SMTP:Port"], out var parsedPort) ? parsedPort : 587;
                bool enableSsl = bool.TryParse(_config["SMTP:EnableSSL"], out var parsedSsl) ? parsedSsl : true;
                var user = _config["SMTP:User"];
                var pass = _config["SMTP:Password"];
                var from = _config["SMTP:From"] ?? user;
                var fromName = _config["SMTP:FromName"] ?? "Music & Mind";
                var adminEmail = _config["SMTP:AdminEmail"] ?? user;

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.EnableSsl = enableSsl;
                    smtp.Credentials = new NetworkCredential(user, pass);

                    var msgCustomer = new MailMessage();
                    msgCustomer.From = new MailAddress(from, fromName);
                    msgCustomer.To.Add(customerEmail);
                    msgCustomer.Subject = "Потвърждение на поръчка";
                    msgCustomer.Body = customerSb.ToString();
                    msgCustomer.IsBodyHtml = false;
                    smtp.Send(msgCustomer);

                    var msgAdmin = new MailMessage();
                    msgAdmin.From = new MailAddress(from, fromName);
                    msgAdmin.To.Add(adminEmail);
                    msgAdmin.Subject = "Нова поръчка (Music & Mind)";
                    msgAdmin.Body = adminSb.ToString();
                    msgAdmin.IsBodyHtml = false;
                    smtp.Send(msgAdmin);
                }
            }
            catch (Exception ex)
            {
                TempData["MailError"] = ex.ToString();
            }

            SaveCart(new List<CartSessionItem>());

            TempData["OrderName"] = name;
            TempData["OrderPayment"] = "Наложен платеж 🚚";

            return RedirectToAction(nameof(OrderSuccess));
        }

        public IActionResult OrderSuccess()
        {
            ViewBag.Name = TempData["OrderName"];
            ViewBag.Payment = TempData["OrderPayment"];
            ViewBag.MailError = TempData["MailError"];
            return View();
        }
    }
}
