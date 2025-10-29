using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace MusicAndMind2.Controllers
{
    public class LangController : Controller
    {
        [HttpPost]
        public IActionResult Set(string culture, string returnUrl = "/")
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }
    }
}