using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Services;
using MovizoneApp.Models;
using MovizoneApp.Enums;

namespace MovizoneApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMovieApplicationService _movieService;
        private readonly ITVSeriesApplicationService _seriesService;
        private readonly IEpisodeService _episodeService;
        private readonly IUserApplicationService _userService;
        private readonly IActorApplicationService _actorService;
        private readonly ISiteSettingsService _settingsService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IMovieApplicationService movieService,
            ITVSeriesApplicationService seriesService,
            IEpisodeService episodeService,
            IUserApplicationService userService,
            IActorApplicationService actorService,
            ISiteSettingsService settingsService,
            ILogger<AdminController> logger)
        {
            _movieService = movieService;
            _seriesService = seriesService;
            _episodeService = episodeService;
            _userService = userService;
            _actorService = actorService;
            _settingsService = settingsService;
            _logger = logger;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString(SessionKeys.UserRole) == UserRole.Admin;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            _logger.LogInformation("Admin accessing dashboard");

            var movies = await _movieService.GetAllMoviesAsync();
            var series = await _seriesService.GetAllSeriesAsync();
            var users = await _userService.GetAllUsersAsync();
            var actors = await _actorService.GetAllActorsAsync();

            var stats = new DashboardStatistics
            {
                TotalMovies = movies.Count(),
                TotalSeries = series.Count(),
                TotalUsers = users.Count(),
                TotalActors = actors.Count(),
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
        public async Task<IActionResult> Login(string email, string password)
        {
            _logger.LogInformation("Admin login attempt for email: {Email}", email);

            var user = await _userService.AuthenticateAsync(email, password);

            if (user != null && user.Role == UserRole.Admin)
            {
                HttpContext.Session.SetString(SessionKeys.UserId, user.Id.ToString());
                HttpContext.Session.SetString(SessionKeys.UserName, user.Name);
                HttpContext.Session.SetString(SessionKeys.UserRole, user.Role);

                _logger.LogInformation("Admin logged in successfully: {Email}", email);
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Failed admin login attempt for email: {Email}", email);
            ViewBag.Error = "Invalid credentials or insufficient permissions";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Movies Management
        public async Task<IActionResult> Movies(string search = "", int page = 1)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin viewing movies list. Search: {Search}, Page: {Page}", search, page);

            var allMovies = await _movieService.GetAllMoviesAsync();
            var movies = allMovies.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                movies = movies.Where(m => m.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                          m.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
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

            try
            {
                await _movieService.CreateMovieAsync(movie);
                _logger.LogInformation("Movie created successfully: {Title}", movie.Title);
                TempData[TempDataKeys.Success] = "Movie created successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie: {Title}", movie.Title);
                TempData[TempDataKeys.Error] = "Failed to create movie. Please try again.";
            }

            return RedirectToAction("Movies");
        }

        public async Task<IActionResult> EditMovie(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing movie ID: {MovieId}", id);

            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie not found for editing: {MovieId}", id);
                return NotFound();
            }

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

            try
            {
                await _movieService.UpdateMovieAsync(movie);
                _logger.LogInformation("Movie updated successfully: {MovieId}", movie.Id);
                TempData[TempDataKeys.Success] = "Movie updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie: {MovieId}", movie.Id);
                TempData[TempDataKeys.Error] = "Failed to update movie. Please try again.";
            }

            return RedirectToAction("Movies");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            try
            {
                await _movieService.DeleteMovieAsync(id);
                _logger.LogInformation("Movie deleted successfully: {MovieId}", id);
                TempData[TempDataKeys.Success] = "Movie deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie: {MovieId}", id);
                TempData[TempDataKeys.Error] = "Failed to delete movie. Please try again.";
            }

            return RedirectToAction("Movies");
        }

        // TV Series Management
        public async Task<IActionResult> Series(string search = "", int page = 1)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin viewing series list. Search: {Search}, Page: {Page}", search, page);

            var allSeries = await _seriesService.GetAllSeriesAsync();
            var series = allSeries.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                series = series.Where(s => s.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                          s.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
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

            try
            {
                await _seriesService.CreateSeriesAsync(series);
                _logger.LogInformation("TV series created successfully: {Title}", series.Title);
                TempData[TempDataKeys.Success] = "TV series created successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TV series: {Title}", series.Title);
                TempData[TempDataKeys.Error] = "Failed to create TV series. Please try again.";
            }

            return RedirectToAction("Series");
        }

        public async Task<IActionResult> EditSeries(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing TV series ID: {SeriesId}", id);

            var series = await _seriesService.GetSeriesByIdAsync(id);
            if (series == null)
            {
                _logger.LogWarning("TV series not found for editing: {SeriesId}", id);
                return NotFound();
            }

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

            try
            {
                await _seriesService.UpdateSeriesAsync(series);
                _logger.LogInformation("TV series updated successfully: {SeriesId}", series.Id);
                TempData[TempDataKeys.Success] = "TV series updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TV series: {SeriesId}", series.Id);
                TempData[TempDataKeys.Error] = "Failed to update TV series. Please try again.";
            }

            return RedirectToAction("Series");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeries(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            try
            {
                await _seriesService.DeleteSeriesAsync(id);
                _logger.LogInformation("TV series deleted successfully: {SeriesId}", id);
                TempData[TempDataKeys.Success] = "TV series deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting TV series: {SeriesId}", id);
                TempData[TempDataKeys.Error] = "Failed to delete TV series. Please try again.";
            }

            return RedirectToAction("Series");
        }

        // Users Management
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin viewing users list");

            var users = await _userService.GetAllUsersAsync();
            return View(users.ToList());
        }

        public async Task<IActionResult> EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing user ID: {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found for editing: {UserId}", id);
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateUserAsync(user);
                    _logger.LogInformation("User updated successfully: {UserId}", user.Id);
                    TempData[TempDataKeys.Success] = "User updated successfully!";
                    return RedirectToAction("Users");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
                    TempData[TempDataKeys.Error] = "Failed to update user. Please try again.";
                }
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            try
            {
                await _userService.DeleteUserAsync(id);
                _logger.LogInformation("User deleted successfully: {UserId}", id);
                TempData[TempDataKeys.Success] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                TempData[TempDataKeys.Error] = "Failed to delete user. Please try again.";
            }

            return RedirectToAction("Users");
        }

        // Actors Management
        public async Task<IActionResult> Actors()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin viewing actors list");

            var actors = await _actorService.GetAllActorsAsync();
            return View(actors.ToList());
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

            try
            {
                await _actorService.CreateActorAsync(actor);
                _logger.LogInformation("Actor created successfully: {Name}", actor.Name);
                TempData[TempDataKeys.Success] = "Actor created successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating actor: {Name}", actor.Name);
                TempData[TempDataKeys.Error] = "Failed to create actor. Please try again.";
            }

            return RedirectToAction("Actors");
        }

        public async Task<IActionResult> EditActor(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing actor ID: {ActorId}", id);

            var actor = await _actorService.GetActorByIdAsync(id);
            if (actor == null)
            {
                _logger.LogWarning("Actor not found for editing: {ActorId}", id);
                return NotFound();
            }

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

            try
            {
                await _actorService.UpdateActorAsync(actor);
                _logger.LogInformation("Actor updated successfully: {ActorId}", actor.Id);
                TempData[TempDataKeys.Success] = "Actor updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating actor: {ActorId}", actor.Id);
                TempData[TempDataKeys.Error] = "Failed to update actor. Please try again.";
            }

            return RedirectToAction("Actors");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteActor(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            try
            {
                await _actorService.DeleteActorAsync(id);
                _logger.LogInformation("Actor deleted successfully: {ActorId}", id);
                TempData[TempDataKeys.Success] = "Actor deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting actor: {ActorId}", id);
                TempData[TempDataKeys.Error] = "Failed to delete actor. Please try again.";
            }

            return RedirectToAction("Actors");
        }

        // Episodes Management
        public async Task<IActionResult> Episodes(int? seriesId, int page = 1)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var allSeries = await _seriesService.GetAllSeriesAsync();
            ViewBag.AllSeries = allSeries;
            ViewBag.SelectedSeriesId = seriesId;

            List<Episode> episodes;
            if (seriesId.HasValue)
            {
                episodes = _episodeService.GetEpisodesBySeriesId(seriesId.Value);
                var selectedSeries = await _seriesService.GetSeriesByIdAsync(seriesId.Value);
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

        public async Task<IActionResult> CreateEpisode(int? seriesId)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            ViewBag.AllSeries = await _seriesService.GetAllSeriesAsync();
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

        public async Task<IActionResult> EditEpisode(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var episode = _episodeService.GetEpisodeById(id);
            if (episode == null) return NotFound();

            ViewBag.AllSeries = await _seriesService.GetAllSeriesAsync();
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
