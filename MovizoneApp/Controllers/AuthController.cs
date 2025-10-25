using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Models;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult SignIn()
        {
            // If already logged in, redirect to profile
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Profile");
            }
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
            var user = _userService.Authenticate(email, password);
            if (user != null)
            {
                // Store user info in session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                TempData["Success"] = $"Welcome back, {user.Name}!";

                // Redirect to admin if admin
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string name, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "All fields are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return View();
            }

            // Check if user already exists
            var existingUser = _userService.GetUserByEmail(email);
            if (existingUser != null)
            {
                TempData["Error"] = "User with this email already exists.";
                return View();
            }

            // Create new user
            var newUser = new User
            {
                Name = name,
                Email = email,
                Password = password, // In production, hash this!
                Role = "User",
                CreatedAt = DateTime.Now
            };

            _userService.AddUser(newUser);
            TempData["Success"] = "Registration successful! Please sign in.";
            return RedirectToAction("SignIn");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("SignIn");
            }

            var user = _userService.GetUserById(userId.Value);
            if (user == null)
            {
                return RedirectToAction("SignIn");
            }

            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
