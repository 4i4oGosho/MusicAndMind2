using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Models;
using Newtonsoft.Json;

namespace MusicAndMind2.Controllers
{
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
        public IActionResult AddToCart(int id)
        {
            var product = ShopController.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            var cart = GetCart();
            cart.Add(product);
            SaveCart(cart);

            // 🔔 Запазваме броя в TempData, за да обновим брояча
            TempData["CartCount"] = cart.Count;

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
    }
}
