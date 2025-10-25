using Microsoft.AspNetCore.Mvc;

namespace MovizoneApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            // Demo authentication - always succeeds
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SignUp(string name, string email, string password)
        {
            // Demo registration - always succeeds
            return RedirectToAction("SignIn");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
