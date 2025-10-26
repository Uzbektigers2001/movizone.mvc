using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;
using MovizoneApp.Models;
using MovizoneApp.Enums;

namespace MovizoneApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ITVSeriesService _seriesService;
        private readonly IEpisodeService _episodeService;
        private readonly IUserService _userService;
        private readonly IActorService _actorService;
        private readonly ISiteSettingsService _settingsService;

        public AdminController(
            IMovieService movieService,
            ITVSeriesService seriesService,
            IEpisodeService episodeService,
            IUserService userService,
            IActorService actorService,
            ISiteSettingsService settingsService)
        {
            _movieService = movieService;
            _seriesService = seriesService;
            _episodeService = episodeService;
            _userService = userService;
            _actorService = actorService;
            _settingsService = settingsService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString(SessionKeys.UserRole) == UserRole.Admin;
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

            if (user != null && user.Role == UserRole.Admin)
            {
                HttpContext.Session.SetString(SessionKeys.UserId, user.Id.ToString());
                HttpContext.Session.SetString(SessionKeys.UserName, user.Name);
                HttpContext.Session.SetString(SessionKeys.UserRole, user.Role);

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
        public IActionResult Movies(string search = "", int page = 1)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var movies = _movieService.GetAllMovies();

            if (!string.IsNullOrEmpty(search))
            {
                movies = movies.Where(m => m.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                          m.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.SearchQuery = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = 15;
            ViewBag.TotalPages = (int)Math.Ceiling(movies.Count() / 15.0);

            var paginatedMovies = movies.Skip((page - 1) * 15).Take(15).ToList();
            return View(paginatedMovies);
        }

        public IActionResult CreateMovie()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(Movie movie, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, IFormFile? videoFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Handle cover image upload
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

            // Handle poster image upload
            if (posterFile != null && posterFile.Length > 0)
            {
                // Create posters directory if it doesn't exist
                var postersDir = Path.Combine("wwwroot/img/posters");
                if (!Directory.Exists(postersDir))
                {
                    Directory.CreateDirectory(postersDir);
                }

                var posterFileName = $"poster_{Guid.NewGuid()}{Path.GetExtension(posterFile.FileName)}";
                var posterPath = Path.Combine(postersDir, posterFileName);

                using (var stream = new FileStream(posterPath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                movie.PosterImage = $"/img/posters/{posterFileName}";
            }

            // Handle banner image upload
            if (bannerFile != null && bannerFile.Length > 0)
            {
                // Create banners directory if it doesn't exist
                var bannersDir = Path.Combine("wwwroot/img/banners");
                if (!Directory.Exists(bannersDir))
                {
                    Directory.CreateDirectory(bannersDir);
                }

                var bannerFileName = $"banner_{Guid.NewGuid()}{Path.GetExtension(bannerFile.FileName)}";
                var bannerPath = Path.Combine(bannersDir, bannerFileName);

                using (var stream = new FileStream(bannerPath, FileMode.Create))
                {
                    await bannerFile.CopyToAsync(stream);
                }

                movie.BannerImage = $"/img/banners/{bannerFileName}";
            }

            // Handle video file upload
            if (videoFile != null && videoFile.Length > 0)
            {
                // Create videos directory if it doesn't exist
                var videosDir = Path.Combine("wwwroot/videos");
                if (!Directory.Exists(videosDir))
                {
                    Directory.CreateDirectory(videosDir);
                }

                var videoFileName = $"video_{Guid.NewGuid()}{Path.GetExtension(videoFile.FileName)}";
                var videoPath = Path.Combine(videosDir, videoFileName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }

                movie.VideoUrl = $"/videos/{videoFileName}";
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
        public async Task<IActionResult> EditMovie(Movie movie, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Handle cover file upload
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

            // Handle poster file upload
            if (posterFile != null && posterFile.Length > 0)
            {
                // Create posters directory if it doesn't exist
                var postersDir = Path.Combine("wwwroot/img/posters");
                if (!Directory.Exists(postersDir))
                {
                    Directory.CreateDirectory(postersDir);
                }

                var posterFileName = $"poster_{Guid.NewGuid()}{Path.GetExtension(posterFile.FileName)}";
                var posterPath = Path.Combine(postersDir, posterFileName);

                using (var stream = new FileStream(posterPath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                movie.PosterImage = $"/img/posters/{posterFileName}";
            }

            // Handle banner file upload
            if (bannerFile != null && bannerFile.Length > 0)
            {
                // Create banners directory if it doesn't exist
                var bannersDir = Path.Combine("wwwroot/img/banners");
                if (!Directory.Exists(bannersDir))
                {
                    Directory.CreateDirectory(bannersDir);
                }

                var bannerFileName = $"banner_{Guid.NewGuid()}{Path.GetExtension(bannerFile.FileName)}";
                var bannerPath = Path.Combine(bannersDir, bannerFileName);

                using (var stream = new FileStream(bannerPath, FileMode.Create))
                {
                    await bannerFile.CopyToAsync(stream);
                }

                movie.BannerImage = $"/img/banners/{bannerFileName}";
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
        public IActionResult Series(string search = "", int page = 1)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var series = _seriesService.GetAllSeries();

            if (!string.IsNullOrEmpty(search))
            {
                series = series.Where(s => s.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                          s.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.SearchQuery = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = 15;
            ViewBag.TotalPages = (int)Math.Ceiling(series.Count() / 15.0);

            var paginatedSeries = series.Skip((page - 1) * 15).Take(15).ToList();
            return View(paginatedSeries);
        }

        public IActionResult CreateSeries()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSeries(TVSeries series, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, string? actorsList)
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

            // Handle poster image upload
            if (posterFile != null && posterFile.Length > 0)
            {
                // Create posters directory if it doesn't exist
                var postersDir = Path.Combine("wwwroot/img/posters");
                if (!Directory.Exists(postersDir))
                {
                    Directory.CreateDirectory(postersDir);
                }

                var posterFileName = $"poster_{Guid.NewGuid()}{Path.GetExtension(posterFile.FileName)}";
                var posterPath = Path.Combine(postersDir, posterFileName);

                using (var stream = new FileStream(posterPath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                series.PosterImage = $"/img/posters/{posterFileName}";
            }

            // Handle banner image upload
            if (bannerFile != null && bannerFile.Length > 0)
            {
                // Create banners directory if it doesn't exist
                var bannersDir = Path.Combine("wwwroot/img/banners");
                if (!Directory.Exists(bannersDir))
                {
                    Directory.CreateDirectory(bannersDir);
                }

                var bannerFileName = $"banner_{Guid.NewGuid()}{Path.GetExtension(bannerFile.FileName)}";
                var bannerPath = Path.Combine(bannersDir, bannerFileName);

                using (var stream = new FileStream(bannerPath, FileMode.Create))
                {
                    await bannerFile.CopyToAsync(stream);
                }

                series.BannerImage = $"/img/banners/{bannerFileName}";
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
        public async Task<IActionResult> EditSeries(TVSeries series, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, string? actorsList)
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

            // Handle poster file upload
            if (posterFile != null && posterFile.Length > 0)
            {
                // Create posters directory if it doesn't exist
                var postersDir = Path.Combine("wwwroot/img/posters");
                if (!Directory.Exists(postersDir))
                {
                    Directory.CreateDirectory(postersDir);
                }

                var posterFileName = $"poster_{Guid.NewGuid()}{Path.GetExtension(posterFile.FileName)}";
                var posterPath = Path.Combine(postersDir, posterFileName);

                using (var stream = new FileStream(posterPath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                series.PosterImage = $"/img/posters/{posterFileName}";
            }

            // Handle banner file upload
            if (bannerFile != null && bannerFile.Length > 0)
            {
                // Create banners directory if it doesn't exist
                var bannersDir = Path.Combine("wwwroot/img/banners");
                if (!Directory.Exists(bannersDir))
                {
                    Directory.CreateDirectory(bannersDir);
                }

                var bannerFileName = $"banner_{Guid.NewGuid()}{Path.GetExtension(bannerFile.FileName)}";
                var bannerPath = Path.Combine(bannersDir, bannerFileName);

                using (var stream = new FileStream(bannerPath, FileMode.Create))
                {
                    await bannerFile.CopyToAsync(stream);
                }

                series.BannerImage = $"/img/banners/{bannerFileName}";
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

        // Episodes Management
        public IActionResult Episodes(int? seriesId, int page = 1)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var allSeries = _seriesService.GetAllSeries();
            ViewBag.AllSeries = allSeries;
            ViewBag.SelectedSeriesId = seriesId;

            List<Episode> episodes;
            if (seriesId.HasValue)
            {
                episodes = _episodeService.GetEpisodesBySeriesId(seriesId.Value);
                var selectedSeries = _seriesService.GetSeriesById(seriesId.Value);
                ViewBag.SelectedSeriesTitle = selectedSeries?.Title;
            }
            else
            {
                episodes = _episodeService.GetAllEpisodes();
            }

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = 15;
            ViewBag.TotalPages = (int)Math.Ceiling(episodes.Count() / 15.0);

            var paginatedEpisodes = episodes.Skip((page - 1) * 15).Take(15).ToList();
            return View(paginatedEpisodes);
        }

        public IActionResult CreateEpisode(int? seriesId)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            ViewBag.AllSeries = _seriesService.GetAllSeries();
            ViewBag.SelectedSeriesId = seriesId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEpisode(Episode episode, IFormFile? thumbnailFile, IFormFile? videoFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Handle thumbnail upload
            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var thumbnailDir = Path.Combine("wwwroot/img/episodes");
                if (!Directory.Exists(thumbnailDir))
                {
                    Directory.CreateDirectory(thumbnailDir);
                }

                var thumbnailFileName = $"episode_{Guid.NewGuid()}{Path.GetExtension(thumbnailFile.FileName)}";
                var thumbnailPath = Path.Combine(thumbnailDir, thumbnailFileName);

                using (var stream = new FileStream(thumbnailPath, FileMode.Create))
                {
                    await thumbnailFile.CopyToAsync(stream);
                }

                episode.ThumbnailImage = $"/img/episodes/{thumbnailFileName}";
            }

            // Handle video upload
            if (videoFile != null && videoFile.Length > 0)
            {
                var videosDir = Path.Combine("wwwroot/videos/episodes");
                if (!Directory.Exists(videosDir))
                {
                    Directory.CreateDirectory(videosDir);
                }

                var videoFileName = $"episode_{Guid.NewGuid()}{Path.GetExtension(videoFile.FileName)}";
                var videoPath = Path.Combine(videosDir, videoFileName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }

                episode.VideoUrl = $"/videos/episodes/{videoFileName}";
            }

            _episodeService.AddEpisode(episode);
            return RedirectToAction("Episodes", new { seriesId = episode.TVSeriesId });
        }

        public IActionResult EditEpisode(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var episode = _episodeService.GetEpisodeById(id);
            if (episode == null) return NotFound();

            ViewBag.AllSeries = _seriesService.GetAllSeries();
            return View(episode);
        }

        [HttpPost]
        public async Task<IActionResult> EditEpisode(Episode episode, IFormFile? thumbnailFile, IFormFile? videoFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Handle thumbnail upload
            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var thumbnailDir = Path.Combine("wwwroot/img/episodes");
                if (!Directory.Exists(thumbnailDir))
                {
                    Directory.CreateDirectory(thumbnailDir);
                }

                var thumbnailFileName = $"episode_{Guid.NewGuid()}{Path.GetExtension(thumbnailFile.FileName)}";
                var thumbnailPath = Path.Combine(thumbnailDir, thumbnailFileName);

                using (var stream = new FileStream(thumbnailPath, FileMode.Create))
                {
                    await thumbnailFile.CopyToAsync(stream);
                }

                episode.ThumbnailImage = $"/img/episodes/{thumbnailFileName}";
            }

            // Handle video upload
            if (videoFile != null && videoFile.Length > 0)
            {
                var videosDir = Path.Combine("wwwroot/videos/episodes");
                if (!Directory.Exists(videosDir))
                {
                    Directory.CreateDirectory(videosDir);
                }

                var videoFileName = $"episode_{Guid.NewGuid()}{Path.GetExtension(videoFile.FileName)}";
                var videoPath = Path.Combine(videosDir, videoFileName);

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }

                episode.VideoUrl = $"/videos/episodes/{videoFileName}";
            }

            _episodeService.UpdateEpisode(episode);
            return RedirectToAction("Episodes", new { seriesId = episode.TVSeriesId });
        }

        [HttpPost]
        public IActionResult DeleteEpisode(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var episode = _episodeService.GetEpisodeById(id);
            int? seriesId = episode?.TVSeriesId;

            _episodeService.DeleteEpisode(id);
            return RedirectToAction("Episodes", new { seriesId = seriesId });
        }

        // Site Settings Management
        public IActionResult Settings()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var settings = _settingsService.GetSettings();
            return View(settings);
        }

        [HttpPost]
        public IActionResult Settings(SiteSettings settings)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _settingsService.UpdateSettings(settings);
            TempData[TempDataKeys.Success] = "Settings updated successfully!";
            return RedirectToAction("Settings");
        }
    }
}
