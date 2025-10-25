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
        public IActionResult CreateMovie(Movie movie)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _movieService.AddMovie(movie);
                return RedirectToAction("Movies");
            }
            return View(movie);
        }

        public IActionResult EditMovie(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var movie = _movieService.GetMovieById(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost]
        public IActionResult EditMovie(Movie movie)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _movieService.UpdateMovie(movie);
                return RedirectToAction("Movies");
            }
            return View(movie);
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
        public IActionResult CreateSeries(TVSeries series)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _seriesService.AddSeries(series);
                return RedirectToAction("Series");
            }
            return View(series);
        }

        public IActionResult EditSeries(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var series = _seriesService.GetSeriesById(id);
            if (series == null) return NotFound();

            return View(series);
        }

        [HttpPost]
        public IActionResult EditSeries(TVSeries series)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _seriesService.UpdateSeries(series);
                return RedirectToAction("Series");
            }
            return View(series);
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
        public IActionResult CreateActor(Actor actor)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _actorService.AddActor(actor);
                return RedirectToAction("Actors");
            }
            return View(actor);
        }

        public IActionResult EditActor(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var actor = _actorService.GetActorById(id);
            if (actor == null) return NotFound();

            return View(actor);
        }

        [HttpPost]
        public IActionResult EditActor(Actor actor)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _actorService.UpdateActor(actor);
                return RedirectToAction("Actors");
            }
            return View(actor);
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
