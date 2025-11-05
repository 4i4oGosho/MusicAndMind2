using Microsoft.AspNetCore.Mvc;

namespace MusicAndMind2.Controllers
{
    // Language endpoint removed — kept stub to avoid 404 if URL is requested.
    public class LangController : Controller
    {
        [HttpGet]
        [HttpPost]
        public IActionResult Set() => NotFound();
    }
}