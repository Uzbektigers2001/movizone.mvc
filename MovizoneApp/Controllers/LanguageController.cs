using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class LanguageController : Controller
    {
        private readonly ILocalizationService _localizationService;

        public LanguageController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl = "/")
        {
            _localizationService.SetCulture(culture);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
