using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.DTOs;
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
        private readonly ICommentService _commentService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<AdminController> _logger;
        private readonly IMapper _mapper;

        public AdminController(
            IMovieApplicationService movieService,
            ITVSeriesApplicationService seriesService,
            IEpisodeService episodeService,
            IUserApplicationService userService,
            IActorApplicationService actorService,
            ISiteSettingsService settingsService,
            ICommentService commentService,
            IReviewService reviewService,
            ILogger<AdminController> logger,
            IMapper mapper)
        {
            _movieService = movieService;
            _seriesService = seriesService;
            _episodeService = episodeService;
            _userService = userService;
            _actorService = actorService;
            _settingsService = settingsService;
            _commentService = commentService;
            _reviewService = reviewService;
            _logger = logger;
            _mapper = mapper;
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

            // Get all data as DTOs
            var moviesDto = await _movieService.GetAllMoviesAsync();
            var seriesDto = await _seriesService.GetAllSeriesAsync();
            var usersDto = await _userService.GetAllUsersAsync();
            var actorsDto = await _actorService.GetAllActorsAsync();
            var comments = _commentService.GetAllComments();
            var reviews = _reviewService.GetAllReviews();

            // Calculate monthly statistics
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var moviesThisMonth = moviesDto.Count(m => m.CreatedAt.Month == currentMonth && m.CreatedAt.Year == currentYear);
            var seriesThisMonth = seriesDto.Count(s => s.CreatedAt.Month == currentMonth && s.CreatedAt.Year == currentYear);
            var itemsAddedThisMonth = moviesThisMonth + seriesThisMonth;

            var moviesPreviousMonth = moviesDto.Count(m => m.CreatedAt.Month == previousMonth && m.CreatedAt.Year == previousMonthYear);
            var seriesPreviousMonth = seriesDto.Count(s => s.CreatedAt.Month == previousMonth && s.CreatedAt.Year == previousMonthYear);
            var itemsAddedPreviousMonth = moviesPreviousMonth + seriesPreviousMonth;
            var itemsAddedChange = itemsAddedThisMonth - itemsAddedPreviousMonth;

            var reviewsThisMonth = reviews.Count(r => r.CreatedAt.Month == currentMonth && r.CreatedAt.Year == currentYear);
            var reviewsPreviousMonth = reviews.Count(r => r.CreatedAt.Month == previousMonth && r.CreatedAt.Year == previousMonthYear);
            var reviewsChange = reviewsThisMonth - reviewsPreviousMonth;

            var subscriptionsThisMonth = usersDto.Count(u => u.CreatedAt.Month == currentMonth && u.CreatedAt.Year == currentYear);
            var subscriptionsPreviousMonth = usersDto.Count(u => u.CreatedAt.Month == previousMonth && u.CreatedAt.Year == previousMonthYear);
            var subscriptionsChange = subscriptionsThisMonth - subscriptionsPreviousMonth;

            // Get top items (by rating) - combining movies and series
            var topMovies = moviesDto.OrderByDescending(m => m.Rating).Take(3)
                .Select(m => new TopItemDto { Id = m.Id, Title = m.Title, Category = "Movie", Rating = m.Rating });
            var topSeries = seriesDto.OrderByDescending(s => s.Rating).Take(2)
                .Select(s => new TopItemDto { Id = s.Id, Title = s.Title, Category = "TV Series", Rating = s.Rating });
            var topItems = topMovies.Concat(topSeries).OrderByDescending(i => i.Rating).Take(5).ToList();

            // Get latest items (by creation date)
            var latestMovies = moviesDto.OrderByDescending(m => m.CreatedAt).Take(3)
                .Select(m => new LatestItemDto { Id = m.Id, Title = m.Title, Category = "Movie", Rating = m.Rating, CreatedAt = m.CreatedAt });
            var latestSeries = seriesDto.OrderByDescending(s => s.CreatedAt).Take(2)
                .Select(s => new LatestItemDto { Id = s.Id, Title = s.Title, Category = "TV Series", Rating = s.Rating, CreatedAt = s.CreatedAt });
            var latestItems = latestMovies.Concat(latestSeries).OrderByDescending(i => i.CreatedAt).Take(5).ToList();

            // Get latest users
            var latestUsers = usersDto.OrderByDescending(u => u.CreatedAt).Take(5)
                .Select(u => new LatestUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Username = u.Email.Split('@')[0], // Extract username from email
                    CreatedAt = u.CreatedAt
                }).ToList();

            // Get latest reviews
            var latestReviews = reviews.OrderByDescending(r => r.CreatedAt).Take(5)
                .Select(r => {
                    var movie = moviesDto.FirstOrDefault(m => m.Id == r.MovieId);
                    return new LatestReviewDto
                    {
                        Id = r.Id,
                        ItemTitle = movie?.Title ?? "Unknown",
                        Author = r.UserName,
                        Rating = r.Rating,
                        MovieId = r.MovieId,
                        CreatedAt = r.CreatedAt
                    };
                }).ToList();

            // Calculate views statistics (mock data for now - would need View tracking implementation)
            var viewsThisMonth = 0; // TODO: Implement view tracking
            var viewsChangePercent = 0.0; // TODO: Calculate from previous month

            var statsDto = new DashboardStatisticsDto
            {
                // Main statistics
                TotalMovies = moviesDto.Count(),
                TotalSeries = seriesDto.Count(),
                TotalUsers = usersDto.Count(),
                TotalActors = actorsDto.Count(),
                TotalComments = comments.Count,
                TotalReviews = reviews.Count,

                // Monthly statistics with real change tracking
                SubscriptionsThisMonth = subscriptionsThisMonth,
                SubscriptionsChange = subscriptionsChange,
                ItemsAddedThisMonth = itemsAddedThisMonth,
                ItemsAddedChange = itemsAddedChange,
                ViewsThisMonth = viewsThisMonth,
                ViewsChangePercent = viewsChangePercent,
                ReviewsThisMonth = reviewsThisMonth,
                ReviewsChange = reviewsChange,

                // Additional statistics
                TodayViews = 0, // TODO: Implement today's view tracking
                MonthlyRevenue = 0, // Subscription/pricing system removed

                // Dashboard widgets
                TopItems = topItems,
                LatestItems = latestItems,
                LatestUsers = latestUsers,
                LatestReviews = latestReviews
            };

            return View(statsDto);
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

            var userDto = await _userService.AuthenticateAsync(email, password);

            if (userDto != null && userDto.Role == UserRole.Admin)
            {
                HttpContext.Session.SetString(SessionKeys.UserId, userDto.Id.ToString());
                HttpContext.Session.SetString(SessionKeys.UserName, userDto.Name);
                HttpContext.Session.SetString(SessionKeys.UserRole, userDto.Role);

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

            var allMoviesDto = await _movieService.GetAllMoviesAsync();
            var allMovies = _mapper.Map<List<Movie>>(allMoviesDto);
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
        public async Task<IActionResult> CreateMovie(CreateMovieDto movieDto, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, IFormFile? videoFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Validate required fields
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Movie creation failed - ModelState invalid");
                TempData[TempDataKeys.Error] = "Please fill in all required fields.";
                return View(movieDto);
            }

            // Validate that either VideoUrl or videoFile is provided (not both required, but at least one)
            if (string.IsNullOrWhiteSpace(movieDto.VideoUrl) && (videoFile == null || videoFile.Length == 0))
            {
                _logger.LogWarning("Movie creation failed - No video content provided");
                TempData[TempDataKeys.Error] = "Please provide either a Video URL or upload a video file.";
                ModelState.AddModelError("VideoUrl", "Either Video URL or video file is required");
                return View(movieDto);
            }

            // Validate that either CoverImage path or coverFile is provided (cover image is essential for display)
            if (string.IsNullOrWhiteSpace(movieDto.CoverImage) && (coverFile == null || coverFile.Length == 0))
            {
                _logger.LogWarning("Movie creation failed - No cover image provided");
                TempData[TempDataKeys.Error] = "Please provide either a Cover Image path or upload a cover image file.";
                ModelState.AddModelError("CoverImage", "Either Cover Image path or cover image file is required");
                return View(movieDto);
            }

            // Handle cover image upload
            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                movieDto.CoverImage = $"/img/covers/{fileName}";
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

                movieDto.PosterImage = $"/img/posters/{posterFileName}";
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

                movieDto.BannerImage = $"/img/banners/{bannerFileName}";
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

                movieDto.VideoUrl = $"/videos/{videoFileName}";
            }

            // Parse actors list
            if (!string.IsNullOrEmpty(actorsList))
            {
                movieDto.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            try
            {
                await _movieService.CreateMovieAsync(movieDto);
                _logger.LogInformation("Movie created successfully: {Title}", movieDto.Title);
                TempData[TempDataKeys.Success] = "Movie created successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie: {Title}", movieDto.Title);
                TempData[TempDataKeys.Error] = "Failed to create movie. Please try again.";
            }

            return RedirectToAction("Movies");
        }

        public async Task<IActionResult> EditMovie(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing movie ID: {MovieId}", id);

            var movieDto = await _movieService.GetMovieByIdAsync(id);
            if (movieDto == null)
            {
                _logger.LogWarning("Movie not found for editing: {MovieId}", id);
                return NotFound();
            }

            var updateDto = _mapper.Map<UpdateMovieDto>(movieDto);
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditMovie(UpdateMovieDto movieDto, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, IFormFile? videoFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Validate required fields
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Movie update failed - ModelState invalid for ID: {MovieId}", movieDto.Id);
                TempData[TempDataKeys.Error] = "Please fill in all required fields.";
                return View(movieDto);
            }

            // Validate that either VideoUrl or videoFile is provided (not both required, but at least one)
            if (string.IsNullOrWhiteSpace(movieDto.VideoUrl) && (videoFile == null || videoFile.Length == 0))
            {
                _logger.LogWarning("Movie update failed - No video content provided for ID: {MovieId}", movieDto.Id);
                TempData[TempDataKeys.Error] = "Please provide either a Video URL or upload a video file.";
                ModelState.AddModelError("VideoUrl", "Either Video URL or video file is required");
                return View(movieDto);
            }

            // Validate that either CoverImage path or coverFile is provided (cover image is essential for display)
            if (string.IsNullOrWhiteSpace(movieDto.CoverImage) && (coverFile == null || coverFile.Length == 0))
            {
                _logger.LogWarning("Movie update failed - No cover image provided for ID: {MovieId}", movieDto.Id);
                TempData[TempDataKeys.Error] = "Please provide either a Cover Image path or upload a cover image file.";
                ModelState.AddModelError("CoverImage", "Either Cover Image path or cover image file is required");
                return View(movieDto);
            }

            // Handle cover file upload
            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                movieDto.CoverImage = $"/img/covers/{fileName}";
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

                movieDto.PosterImage = $"/img/posters/{posterFileName}";
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

                movieDto.BannerImage = $"/img/banners/{bannerFileName}";
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

                movieDto.VideoUrl = $"/videos/{videoFileName}";
            }

            // Parse actors list
            if (!string.IsNullOrEmpty(actorsList))
            {
                movieDto.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            try
            {
                await _movieService.UpdateMovieAsync(movieDto);
                _logger.LogInformation("Movie updated successfully: {MovieId}", movieDto.Id);
                TempData[TempDataKeys.Success] = "Movie updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie: {MovieId}", movieDto.Id);
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

            var allSeriesDto = await _seriesService.GetAllSeriesAsync();
            var allSeries = _mapper.Map<List<TVSeries>>(allSeriesDto);
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
        public async Task<IActionResult> CreateSeries(CreateTVSeriesDto seriesDto, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Validate required fields
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("TV series creation failed - ModelState invalid");
                TempData[TempDataKeys.Error] = "Please fill in all required fields.";
                return View(seriesDto);
            }

            // Validate that either CoverImage path or coverFile is provided (cover image is essential for display)
            if (string.IsNullOrWhiteSpace(seriesDto.CoverImage) && (coverFile == null || coverFile.Length == 0))
            {
                _logger.LogWarning("TV series creation failed - No cover image provided");
                TempData[TempDataKeys.Error] = "Please provide either a Cover Image path or upload a cover image file.";
                ModelState.AddModelError("CoverImage", "Either Cover Image path or cover image file is required");
                return View(seriesDto);
            }

            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                seriesDto.CoverImage = $"/img/covers/{fileName}";
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

                seriesDto.PosterImage = $"/img/posters/{posterFileName}";
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

                seriesDto.BannerImage = $"/img/banners/{bannerFileName}";
            }

            if (!string.IsNullOrEmpty(actorsList))
            {
                seriesDto.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            try
            {
                await _seriesService.CreateSeriesAsync(seriesDto);
                _logger.LogInformation("TV series created successfully: {Title}", seriesDto.Title);
                TempData[TempDataKeys.Success] = "TV series created successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TV series: {Title}", seriesDto.Title);
                TempData[TempDataKeys.Error] = "Failed to create TV series. Please try again.";
            }

            return RedirectToAction("Series");
        }

        public async Task<IActionResult> EditSeries(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing TV series ID: {SeriesId}", id);

            var seriesDto = await _seriesService.GetSeriesByIdAsync(id);
            if (seriesDto == null)
            {
                _logger.LogWarning("TV series not found for editing: {SeriesId}", id);
                return NotFound();
            }

            var updateDto = _mapper.Map<UpdateTVSeriesDto>(seriesDto);
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditSeries(UpdateTVSeriesDto seriesDto, IFormFile? coverFile, IFormFile? posterFile, IFormFile? bannerFile, string? actorsList)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Validate required fields
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("TV series update failed - ModelState invalid for ID: {SeriesId}", seriesDto.Id);
                TempData[TempDataKeys.Error] = "Please fill in all required fields.";
                return View(seriesDto);
            }

            // Validate that either CoverImage path or coverFile is provided (cover image is essential for display)
            if (string.IsNullOrWhiteSpace(seriesDto.CoverImage) && (coverFile == null || coverFile.Length == 0))
            {
                _logger.LogWarning("TV series update failed - No cover image provided for ID: {SeriesId}", seriesDto.Id);
                TempData[TempDataKeys.Error] = "Please provide either a Cover Image path or upload a cover image file.";
                ModelState.AddModelError("CoverImage", "Either Cover Image path or cover image file is required");
                return View(seriesDto);
            }

            if (coverFile != null && coverFile.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
                var filePath = Path.Combine("wwwroot/img/covers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverFile.CopyToAsync(stream);
                }

                seriesDto.CoverImage = $"/img/covers/{fileName}";
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

                seriesDto.PosterImage = $"/img/posters/{posterFileName}";
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

                seriesDto.BannerImage = $"/img/banners/{bannerFileName}";
            }

            if (!string.IsNullOrEmpty(actorsList))
            {
                seriesDto.Actors = actorsList.Split(',').Select(a => a.Trim()).ToList();
            }

            try
            {
                await _seriesService.UpdateSeriesAsync(seriesDto);
                _logger.LogInformation("TV series updated successfully: {SeriesId}", seriesDto.Id);
                TempData[TempDataKeys.Success] = "TV series updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TV series: {SeriesId}", seriesDto.Id);
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

            var usersDto = await _userService.GetAllUsersAsync();
            var users = _mapper.Map<List<User>>(usersDto);
            return View(users);
        }

        public async Task<IActionResult> EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing user ID: {UserId}", id);

            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                _logger.LogWarning("User not found for editing: {UserId}", id);
                return NotFound();
            }

            var updateDto = _mapper.Map<UpdateUserDto>(userDto);
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UpdateUserDto userDto)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateUserAsync(userDto);
                    _logger.LogInformation("User updated successfully: {UserId}", userDto.Id);
                    TempData[TempDataKeys.Success] = "User updated successfully!";
                    return RedirectToAction("Users");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating user: {UserId}", userDto.Id);
                    TempData[TempDataKeys.Error] = "Failed to update user. Please try again.";
                }
            }
            return View(userDto);
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

            var actorsDto = await _actorService.GetAllActorsAsync();
            var actors = _mapper.Map<List<Actor>>(actorsDto);
            return View(actors);
        }

        public IActionResult CreateActor()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateActor(CreateActorDto actorDto, IFormFile? photoFile, string? moviesList, string? seriesList)
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

                actorDto.Photo = $"/img/covers/{fileName}";
            }

            if (!string.IsNullOrEmpty(moviesList))
            {
                actorDto.Movies = moviesList.Split(',').Select(m => m.Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(seriesList))
            {
                actorDto.TVSeries = seriesList.Split(',').Select(s => s.Trim()).ToList();
            }

            try
            {
                await _actorService.CreateActorAsync(actorDto);
                _logger.LogInformation("Actor created successfully: {Name}", actorDto.Name);
                TempData[TempDataKeys.Success] = "Actor created successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating actor: {Name}", actorDto.Name);
                TempData[TempDataKeys.Error] = "Failed to create actor. Please try again.";
            }

            return RedirectToAction("Actors");
        }

        public async Task<IActionResult> EditActor(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _logger.LogInformation("Admin editing actor ID: {ActorId}", id);

            var actorDto = await _actorService.GetActorByIdAsync(id);
            if (actorDto == null)
            {
                _logger.LogWarning("Actor not found for editing: {ActorId}", id);
                return NotFound();
            }

            var updateDto = _mapper.Map<UpdateActorDto>(actorDto);
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditActor(UpdateActorDto actorDto, IFormFile? photoFile, string? moviesList, string? seriesList)
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

                actorDto.Photo = $"/img/covers/{fileName}";
            }

            if (!string.IsNullOrEmpty(moviesList))
            {
                actorDto.Movies = moviesList.Split(',').Select(m => m.Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(seriesList))
            {
                actorDto.TVSeries = seriesList.Split(',').Select(s => s.Trim()).ToList();
            }

            try
            {
                await _actorService.UpdateActorAsync(actorDto);
                _logger.LogInformation("Actor updated successfully: {ActorId}", actorDto.Id);
                TempData[TempDataKeys.Success] = "Actor updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating actor: {ActorId}", actorDto.Id);
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

            var allSeriesDto = await _seriesService.GetAllSeriesAsync();
            var allSeries = _mapper.Map<List<TVSeries>>(allSeriesDto);
            ViewBag.AllSeries = allSeries;
            ViewBag.SelectedSeriesId = seriesId;

            List<Episode> episodes;
            if (seriesId.HasValue)
            {
                episodes = _episodeService.GetEpisodesBySeriesId(seriesId.Value);
                var selectedSeriesDto = await _seriesService.GetSeriesByIdAsync(seriesId.Value);
                ViewBag.SelectedSeriesTitle = selectedSeriesDto?.Title;
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

            var allSeriesDto = await _seriesService.GetAllSeriesAsync();
            var allSeries = _mapper.Map<List<TVSeries>>(allSeriesDto);
            ViewBag.AllSeries = allSeries;
            ViewBag.SelectedSeriesId = seriesId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEpisode(CreateEpisodeDto episodeDto, IFormFile? thumbnailFile, IFormFile? videoFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Validate that either VideoUrl or videoFile is provided (not both required, but at least one)
            if (string.IsNullOrWhiteSpace(episodeDto.VideoUrl) && (videoFile == null || videoFile.Length == 0))
            {
                _logger.LogWarning("Episode creation failed - No video content provided");
                TempData[TempDataKeys.Error] = "Please provide either a Video URL or upload a video file.";

                // Re-populate ViewBag for form
                var allSeriesDto = await _seriesService.GetAllSeriesAsync();
                var allSeries = _mapper.Map<List<TVSeries>>(allSeriesDto);
                ViewBag.AllSeries = allSeries;

                return View(episodeDto);
            }

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

                episodeDto.ThumbnailImage = $"/img/episodes/{thumbnailFileName}";
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

                episodeDto.VideoUrl = $"/videos/episodes/{videoFileName}";
            }

            var episode = _mapper.Map<Episode>(episodeDto);
            _episodeService.AddEpisode(episode);
            return RedirectToAction("Episodes", new { seriesId = episodeDto.TVSeriesId });
        }

        public async Task<IActionResult> EditEpisode(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var episode = _episodeService.GetEpisodeById(id);
            if (episode == null) return NotFound();

            var allSeriesDto = await _seriesService.GetAllSeriesAsync();
            var allSeries = _mapper.Map<List<TVSeries>>(allSeriesDto);
            ViewBag.AllSeries = allSeries;

            var updateDto = _mapper.Map<UpdateEpisodeDto>(episode);
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditEpisode(UpdateEpisodeDto episodeDto, IFormFile? thumbnailFile, IFormFile? videoFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Validate that either VideoUrl or videoFile is provided (not both required, but at least one)
            if (string.IsNullOrWhiteSpace(episodeDto.VideoUrl) && (videoFile == null || videoFile.Length == 0))
            {
                _logger.LogWarning("Episode update failed - No video content provided for ID: {EpisodeId}", episodeDto.Id);
                TempData[TempDataKeys.Error] = "Please provide either a Video URL or upload a video file.";

                // Re-populate ViewBag for form
                var allSeriesDto = await _seriesService.GetAllSeriesAsync();
                var allSeries = _mapper.Map<List<TVSeries>>(allSeriesDto);
                ViewBag.AllSeries = allSeries;

                return View(episodeDto);
            }

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

                episodeDto.ThumbnailImage = $"/img/episodes/{thumbnailFileName}";
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

                episodeDto.VideoUrl = $"/videos/episodes/{videoFileName}";
            }

            var episode = _mapper.Map<Episode>(episodeDto);
            _episodeService.UpdateEpisode(episode);
            return RedirectToAction("Episodes", new { seriesId = episodeDto.TVSeriesId });
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

        // Comments Management
        public IActionResult Comments(string search = "")
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var comments = _commentService.GetAllComments();

            if (!string.IsNullOrEmpty(search))
            {
                comments = comments.Where(c =>
                    c.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Text.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.SearchQuery = search;
            return View(comments);
        }

        [HttpPost]
        public IActionResult ApproveComment(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _commentService.ApproveComment(id);
            TempData[TempDataKeys.Success] = "Comment approved successfully!";
            return RedirectToAction("Comments");
        }

        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _commentService.DeleteComment(id);
            TempData[TempDataKeys.Success] = "Comment deleted successfully!";
            return RedirectToAction("Comments");
        }

        // Reviews Management
        public IActionResult Reviews(string search = "")
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var reviews = _reviewService.GetAllReviews();

            if (!string.IsNullOrEmpty(search))
            {
                reviews = reviews.Where(r =>
                    r.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    r.Comment.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.SearchQuery = search;
            return View(reviews);
        }

        [HttpPost]
        public IActionResult DeleteReview(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            _reviewService.DeleteReview(id);
            TempData[TempDataKeys.Success] = "Review deleted successfully!";
            return RedirectToAction("Reviews");
        }
    }
}
