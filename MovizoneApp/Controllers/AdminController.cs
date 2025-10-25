using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ITVSeriesService _seriesService;
        private readonly IUserService _userService;
        private readonly IActorService _actorService;

        public AdminController(
            IMovieService movieService,
            ITVSeriesService seriesService,
            IUserService userService,
            IActorService actorService)
        {
            _movieService = movieService;
            _seriesService = seriesService;
            _userService = userService;
            _actorService = actorService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            var stats = new DashboardStatistics
            {
                TotalMovies = _movieService.GetAllMovies().Count,
                TotalSeries = _seriesService.GetAllSeries().Count,
                TotalUsers = _userService.GetAllUsers().Count,
                TotalActors = _actorService.GetAllActors().Count,
                TodayViews = 1247,
                MonthlyViews = 45231,
                MonthlyRevenue = 12500.50m,
                RecentActivities = new List<RecentActivity>
                {
                    new RecentActivity { Type = "movie", Description = "New movie added", Timestamp = DateTime.Now.AddHours(-2), Icon = "ti-movie" },
                    new RecentActivity { Type = "user", Description = "New user registered", Timestamp = DateTime.Now.AddHours(-5), Icon = "ti-user" },
                    new RecentActivity { Type = "series", Description = "TV series updated", Timestamp = DateTime.Now.AddHours(-8), Icon = "ti-device-tv" }
                }
            };

            return View(stats);
        }

        public IActionResult Login()
        {
            if (IsAdmin())
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _userService.Authenticate(email, password);

            if (user != null && user.Role == "Admin")
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid credentials or insufficient permissions";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Movies Management
        public IActionResult Movies()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var movies = _movieService.GetAllMovies();
            return View(movies);
        }

        public IActionResult CreateMovie()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(Movie movie, IFormFile? coverFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Handle file upload
            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                movie.CoverImage = $"/img/covers/{fileName}";
            }

            // Parse actors list
            if (!string.IsNullOrEmpty(actorsList))
            {
                movie.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            _movieService.AddMovie(movie);
            return RedirectToAction("Movies");
        }

        public IActionResult EditMovie(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var movie = _movieService.GetMovieById(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> EditMovie(Movie movie, IFormFile? coverFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Handle file upload
            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                movie.CoverImage = $"/img/covers/{fileName}";
            }

            // Parse actors list
            if (!string.IsNullOrEmpty(actorsList))
            {
                movie.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            _movieService.UpdateMovie(movie);
            return RedirectToAction("Movies");
        }

        [HttpPost]
        public IActionResult DeleteMovie(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _movieService.DeleteMovie(id);
            return RedirectToAction("Movies");
        }

        // TV Series Management
        public IActionResult Series()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var series = _seriesService.GetAllSeries();
            return View(series);
        }

        public IActionResult CreateSeries()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSeries(TVSeries series, IFormFile? coverFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                series.CoverImage = $"/img/covers/{fileName}";
            }

            if (!string.IsNullOrEmpty(actorsList))
            {
                series.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            _seriesService.AddSeries(series);
            return RedirectToAction("Series");
        }

        public IActionResult EditSeries(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var series = _seriesService.GetSeriesById(id);
            if (series == null) return NotFound();

            return View(series);
        }

        [HttpPost]
        public async Task<IActionResult> EditSeries(TVSeries series, IFormFile? coverFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                series.CoverImage = $"/img/covers/{fileName}";
            }

            if (!string.IsNullOrEmpty(actorsList))
            {
                series.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            _seriesService.UpdateSeries(series);
            return RedirectToAction("Series");
        }

        [HttpPost]
        public IActionResult DeleteSeries(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _seriesService.DeleteSeries(id);
            return RedirectToAction("Series");
        }

        // Users Management
        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var users = _userService.GetAllUsers();
            return View(users);
        }

        public IActionResult EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var user = _userService.GetUserById(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _userService.UpdateUser(user);
                return RedirectToAction("Users");
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _userService.DeleteUser(id);
            return RedirectToAction("Users");
        }

        // Actors Management
        public IActionResult Actors()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var actors = _actorService.GetAllActors();
            return View(actors);
        }

        public IActionResult CreateActor()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateActor(Actor actor, IFormFile? photoFile, string? moviesList, string? seriesList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (photoFile != null && photoFile.Length > 0)
            {
                var fileName = $"actor_{Guid.NewGuid()}{Path.GetExtension(photoFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }

                actor.Photo = $"/img/covers/{fileName}";
            }

            if (!string.IsNullOrEmpty(moviesList))
            {
                actor.Movies = moviesList.Split(',').Select(m => m.Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(seriesList))
            {
                actor.TVSeries = seriesList.Split(',').Select(s => s.Trim()).ToList();
            }

            _actorService.AddActor(actor);
            return RedirectToAction("Actors");
        }

        public IActionResult EditActor(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var actor = _actorService.GetActorById(id);
            if (actor == null) return NotFound();

            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> EditActor(Actor actor, IFormFile? photoFile, string? moviesList, string? seriesList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (photoFile != null && photoFile.Length > 0)
            {
                var fileName = $"actor_{Guid.NewGuid()}{Path.GetExtension(photoFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }

                actor.Photo = $"/img/covers/{fileName}";
            }

            if (!string.IsNullOrEmpty(moviesList))
            {
                actor.Movies = moviesList.Split(',').Select(m => m.Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(seriesList))
            {
                actor.TVSeries = seriesList.Split(',').Select(s => s.Trim()).ToList();
            }

            _actorService.UpdateActor(actor);
            return RedirectToAction("Actors");
        }

        [HttpPost]
        public IActionResult DeleteActor(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _actorService.DeleteActor(id);
            return RedirectToAction("Actors");
        }
    }
}
