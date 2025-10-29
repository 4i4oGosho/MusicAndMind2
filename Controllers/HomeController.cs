using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MusicAndMind2.Controllers
{
    public class HomeController(IStringLocalizerFactory factory) : Controller
    {
        private readonly IStringLocalizer _L = factory.Create("SharedResource", typeof(Program).Assembly.FullName!);

        public IActionResult Index(){ ViewData["Title"] = _L["AppTitle"]; return View(); }
        public IActionResult Focus(){ ViewData["Title"] = _L["Focus_Title"]; return View(); }
        public IActionResult Science(){ ViewData["Title"] = _L["Science_Title"]; return View(); }
        public IActionResult About(){ ViewData["Title"] = _L["About_Title"]; return View(); }
        public IActionResult Contact(){ ViewData["Title"] = _L["Contact_Title"]; return View(); }
    }
}