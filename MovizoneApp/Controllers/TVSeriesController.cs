using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Models;
using MovizoneApp.Services;
using MovizoneApp.Helpers;

namespace MovizoneApp.Controllers
{
    public class TVSeriesController : Controller
    {
        private readonly ITVSeriesService _tvSeriesService;
        private readonly IReviewService _reviewService;
        private readonly IWatchlistService _watchlistService;

        public TVSeriesController(ITVSeriesService tvSeriesService, IReviewService reviewService, IWatchlistService watchlistService)
        {
            _tvSeriesService = tvSeriesService;
            _reviewService = reviewService;
            _watchlistService = watchlistService;
        }

        public IActionResult Catalog(string search = "", string genre = "", int pageNumber = 1)
        {
            var series = _tvSeriesService.GetAllSeries().Where(s => !s.IsHidden).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                series = series.Where(s => s.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                          s.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                series = series.Where(s => s.Genre == genre).ToList();
            }

            ViewBag.SearchQuery = search;
            ViewBag.SelectedGenre = genre;
            ViewBag.Genres = _tvSeriesService.GetAllSeries().Where(s => !s.IsHidden).Select(s => s.Genre).Distinct().OrderBy(g => g).ToList();

            int pageSize = 12;
            var paginatedSeries = PaginatedList<TVSeries>.Create(series, pageNumber, pageSize);

            return View(paginatedSeries);
        }

        public IActionResult Details(int id)
        {
            var series = _tvSeriesService.GetSeriesById(id);
            if (series == null)
            {
                return NotFound();
            }

            var reviews = _reviewService.GetReviewsByMovieId(id);
            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            ViewBag.ReviewCount = reviews.Count;

            // Check if in watchlist (userId = 1 for demo)
            ViewBag.IsInWatchlist = _watchlistService.IsInWatchlist(1, id);

            // Get similar series based on genre (exclude current series and hidden series)
            var similarSeries = _tvSeriesService.GetAllSeries()
                .Where(s => s.Id != id && !s.IsHidden && s.Genre == series.Genre)
                .OrderByDescending(s => s.Rating)
                .Take(6)
                .ToList();
            ViewBag.SimilarSeries = similarSeries;

            return View(series);
        }

        [HttpPost]
        public IActionResult AddReview(int seriesId, string userName, string comment, int rating)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(comment))
            {
                TempData["Error"] = "Please fill in all fields";
                return RedirectToAction("Details", new { id = seriesId });
            }

            var review = new Review
            {
                MovieId = seriesId, // Using MovieId field for SeriesId
                UserId = 1,
                UserName = userName,
                Comment = comment,
                Rating = rating
            };

            _reviewService.AddReview(review);
            TempData["Success"] = "Review added successfully!";
            return RedirectToAction("Details", new { id = seriesId });
        }
    }
}
