using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Models;
using Newtonsoft.Json;

namespace MusicAndMind2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        // 🔐 ВАЖНО: Отделна кошница за всеки потребител
        private string CartSessionKey =>
            User.Identity!.IsAuthenticated
                ? $"CartItems_{User.Identity.Name}"   // За логнат потребител
                : "CartItems_Guest";                  // За гост

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

        // ➕ Добавяне на продукт
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

        // 🧾 Checkout
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            ViewBag.Cart = cart;
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(string name, string address, string city, string phone, string paymentMethod)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            // 🗑 Изчистваме кошницата след поръчка
            SaveCart(new List<Product>());

            TempData["OrderName"] = name;
            TempData["OrderPayment"] = paymentMethod == "card" ? "Карта 💳" : "Наложен платеж 🚚";

            return RedirectToAction(nameof(OrderSuccess));
        }

        // 🎉 Успешна поръчка
        public IActionResult OrderSuccess()
        {
            ViewBag.Name = TempData["OrderName"];
            ViewBag.Payment = TempData["OrderPayment"];
            return View();
        }
    }
}
