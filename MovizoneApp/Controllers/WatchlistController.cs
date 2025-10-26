using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;

namespace MovizoneApp.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly IWatchlistApplicationService _watchlistService;
        private readonly IMovieApplicationService _movieService;
        private readonly ITVSeriesApplicationService _tvSeriesService;
        private readonly ILogger<WatchlistController> _logger;

        public WatchlistController(
            IWatchlistApplicationService watchlistService,
            IMovieApplicationService movieService,
            ITVSeriesApplicationService tvSeriesService,
            ILogger<WatchlistController> logger)
        {
            _watchlistService = watchlistService;
            _movieService = movieService;
            _tvSeriesService = tvSeriesService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _logger.LogInformation("Fetching watchlist for user {UserId}", userId);

            var watchlistItems = await _watchlistService.GetUserWatchlistAsync(userId);

            // Fetch movie details for each watchlist item
            var movies = new List<object>();
            foreach (var item in watchlistItems)
            {
                var movie = await _movieService.GetMovieByIdAsync(item.MovieId);
                if (movie != null)
                {
                    movies.Add(new
                    {
                        WatchlistId = item.Id,
                        MovieId = movie.Id,
                        Title = movie.Title,
                        CoverImage = movie.CoverImage,
                        Rating = movie.Rating,
                        Year = movie.Year,
                        Genre = movie.Genre,
                        AddedAt = item.AddedAt,
                        Type = "Movie"
                    });
                }
                else
                {
                    var series = await _tvSeriesService.GetSeriesByIdAsync(item.MovieId);
                    if (series != null)
                    {
                        movies.Add(new
                        {
                            WatchlistId = item.Id,
                            MovieId = series.Id,
                            Title = series.Title,
                            CoverImage = series.CoverImage,
                            Rating = series.Rating,
                            Year = series.Year,
                            Genre = series.Genre,
                            AddedAt = item.AddedAt,
                            Type = "Series"
                        });
                    }
                }
            }

            ViewBag.WatchlistItems = movies;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(int movieId, string returnUrl)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _logger.LogInformation("Adding movie {MovieId} to watchlist for user {UserId}", movieId, userId);

            try
            {
                await _watchlistService.AddToWatchlistAsync(userId, movieId);
                TempData["Success"] = "Added to your watchlist!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to watchlist for user {UserId}", userId);
                TempData["Error"] = "Failed to add to watchlist.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int movieId, string returnUrl)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", movieId, userId);

            try
            {
                await _watchlistService.RemoveFromWatchlistAsync(userId, movieId);
                TempData["Success"] = "Removed from your watchlist!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from watchlist for user {UserId}", userId);
                TempData["Error"] = "Failed to remove from watchlist.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> IsInWatchlist(int movieId)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(userId, movieId);
            return Json(new { isInWatchlist });
        }
    }
}
