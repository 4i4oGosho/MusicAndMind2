using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Models;
using Newtonsoft.Json;

namespace MusicAndMind2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private const string CartSessionKey = "CartItems";

        // 🧠 Взимаме текущата кошница от сесията
        private List<Product> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson != null
                ? JsonConvert.DeserializeObject<List<Product>>(cartJson) ?? new List<Product>()
                : new List<Product>();
        }

        // 💾 Запазваме кошницата обратно в сесията
        private void SaveCart(List<Product> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        // 🛍️ Добавяне на продукт в кошницата
        [HttpPost]
        [IgnoreAntiforgeryToken] // ⬅️ добави този ред
        public IActionResult AddToCart(int id)

        {
            var product = ShopController.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            var cart = GetCart();
            cart.Add(product);
            SaveCart(cart);

            TempData["CartCount"] = cart.Count;

            // ⚡ Ако заявката идва чрез fetch (AJAX), връщаме JSON вместо redirect
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, count = cart.Count });
            }

            // ⚡ Ако идва от страницата с детайли — връщаме обратно към продукта
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && referer.Contains("/Shop/Details"))
                return Redirect(referer);

            // В противен случай — към кошницата
            return RedirectToAction("Index");
        }

        // 🧺 Преглед на кошницата
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.CartCount = cart.Count;
            return View(cart);
        }

        // ❌ Премахване на продукт от кошницата
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
            return RedirectToAction("Index");
        }

        // 🧹 Изчистване на кошницата
        [HttpPost]
        public IActionResult ClearCart()
        {
            SaveCart(new List<Product>());
            return RedirectToAction("Index");
        }

        // 🧾 Страница за завършване на поръчката
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            ViewBag.Cart = cart;
            return View();
        }

        // ✅ Потвърждение на поръчката
        [HttpPost]
        public IActionResult Checkout(string name, string address, string city, string phone, string paymentMethod)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            SaveCart(new List<Product>()); // Изчистваме кошницата

            TempData["OrderName"] = name;
            TempData["OrderPayment"] = paymentMethod == "card" ? "Карта 💳" : "Наложен платеж 🚚";

            return RedirectToAction("OrderSuccess");
        }

        // 🎉 Страница за успех
        [HttpGet]
        public IActionResult OrderSuccess()
        {
            ViewBag.Name = TempData["OrderName"];
            ViewBag.Payment = TempData["OrderPayment"];
            return View();
        }
    }
}
