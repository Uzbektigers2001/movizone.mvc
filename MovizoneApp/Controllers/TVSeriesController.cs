using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class TVSeriesController : Controller
    {
        private readonly ITVSeriesApplicationService _tvSeriesService;
        private readonly IReviewApplicationService _reviewService;
        private readonly IWatchlistApplicationService _watchlistService;
        private readonly ILogger<TVSeriesController> _logger;

        public TVSeriesController(
            ITVSeriesApplicationService tvSeriesService,
            IReviewApplicationService reviewService,
            IWatchlistApplicationService watchlistService,
            ILogger<TVSeriesController> logger)
        {
            _tvSeriesService = tvSeriesService;
            _reviewService = reviewService;
            _watchlistService = watchlistService;
            _logger = logger;
        }

        public async Task<IActionResult> Catalog(string search = "", string genre = "")
        {
            _logger.LogInformation("Accessing TV series catalog. Search: {Search}, Genre: {Genre}", search, genre);

            var series = await _tvSeriesService.SearchSeriesAsync(search, genre);
            var genres = await _tvSeriesService.GetAllGenresAsync();

            ViewBag.SearchQuery = search;
            ViewBag.SelectedGenre = genre;
            ViewBag.Genres = genres;

            return View(series);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Accessing TV series details for ID: {SeriesId}", id);

            var series = await _tvSeriesService.GetSeriesByIdAsync(id);
            if (series == null)
            {
                _logger.LogWarning("TV series not found: {SeriesId}", id);
                return NotFound();
            }

            var reviews = await _reviewService.GetReviewsByTVSeriesIdAsync(id);
            var averageRating = await _reviewService.GetAverageRatingForTVSeriesAsync(id);
            var reviewCount = await _reviewService.GetReviewCountForTVSeriesAsync(id);
            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(1, id); // userId = 1 for demo

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.IsInWatchlist = isInWatchlist;

            // Get similar series based on genre (exclude current series)
            var allSeries = await _tvSeriesService.GetAllSeriesAsync();
            var similarSeries = allSeries
                .Where(s => s.Id != id && s.Genre == series.Genre)
                .OrderByDescending(s => s.Rating)
                .Take(6)
                .ToList();
            ViewBag.SimilarSeries = similarSeries;

            return View(series);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int seriesId, string userName, string comment, int rating)
        {
            _logger.LogInformation("Adding review for TV series ID: {SeriesId}", seriesId);

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(comment))
            {
                TempData["Error"] = "Please fill in all fields";
                return RedirectToAction("Details", new { id = seriesId });
            }

            try
            {
                var review = new Review
                {
                    TVSeriesId = seriesId, // Fixed: using TVSeriesId instead of MovieId
                    UserId = 1,
                    UserName = userName,
                    Comment = comment,
                    Rating = rating
                };

                await _reviewService.AddReviewAsync(review);
                TempData["Success"] = "Review added successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review for TV series {SeriesId}", seriesId);
                TempData["Error"] = "Failed to add review. Please try again.";
            }

            return RedirectToAction("Details", new { id = seriesId });
        }
    }
}
