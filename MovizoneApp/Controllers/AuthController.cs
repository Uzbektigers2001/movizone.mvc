using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserApplicationService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserApplicationService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
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
        public async Task<IActionResult> SignIn(string email, string password)
        {
            _logger.LogInformation("User sign in attempt: {Email}", email);

            var user = await _userService.AuthenticateAsync(email, password);
            if (user != null)
            {
                // Store user info in session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                _logger.LogInformation("User signed in successfully: {Email}", email);
                TempData["Success"] = $"Welcome back, {user.Name}!";

                // Redirect to admin if admin
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Profile");
            }

            _logger.LogWarning("Failed sign in attempt: {Email}", email);
            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string name, string email, string password, string confirmPassword)
        {
            _logger.LogInformation("User sign up attempt: {Email}", email);

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
            var existingUser = await _userService.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning("Sign up failed - email already exists: {Email}", email);
                TempData["Error"] = "User with this email already exists.";
                return View();
            }

            // Create new user
            var newUser = new User
            {
                Name = name,
                Email = email,
                Password = password,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _userService.CreateUserAsync(newUser, password);
                _logger.LogInformation("User registered successfully: {Email}", email);
                TempData["Success"] = "Registration successful! Please sign in.";
                return RedirectToAction("SignIn");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration: {Email}", email);
                TempData["Error"] = "Registration failed. Please try again.";
                return View();
            }
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("SignIn");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                _logger.LogWarning("User not found for profile: {UserId}", userId.Value);
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
