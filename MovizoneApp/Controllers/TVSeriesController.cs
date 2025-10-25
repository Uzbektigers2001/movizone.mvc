using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Models;
using MovizoneApp.Services;

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

        public IActionResult Catalog(string search = "", string genre = "")
        {
            var series = _tvSeriesService.GetAllSeries();

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
            ViewBag.Genres = _tvSeriesService.GetAllSeries().Select(s => s.Genre).Distinct().OrderBy(g => g).ToList();
            return View(series);
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
