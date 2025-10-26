using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Helpers;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class TVSeriesController : Controller
    {
        private readonly ITVSeriesApplicationService _tvSeriesService;
        private readonly IReviewApplicationService _reviewService;
        private readonly IWatchlistApplicationService _watchlistService;
        private readonly ILogger<TVSeriesController> _logger;
        private readonly IMapper _mapper;

        public TVSeriesController(
            ITVSeriesApplicationService tvSeriesService,
            IReviewApplicationService reviewService,
            IWatchlistApplicationService watchlistService,
            ILogger<TVSeriesController> logger,
            IMapper mapper)
        {
            _tvSeriesService = tvSeriesService;
            _reviewService = reviewService;
            _watchlistService = watchlistService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Catalog(string search = "", string genre = "", int page = 1)
        {
            _logger.LogInformation("Accessing TV series catalog. Search: {Search}, Genre: {Genre}, Page: {Page}", search, genre, page);

            var allSeriesDto = await _tvSeriesService.SearchSeriesAsync(search, genre);
            var allSeries = _mapper.Map<IEnumerable<TVSeries>>(allSeriesDto);
            var genres = await _tvSeriesService.GetAllGenresAsync();

            // Create paginated list
            const int pageSize = 12;
            var paginatedSeries = PaginatedList<TVSeries>.Create(allSeries, page, pageSize);

            ViewBag.SearchQuery = search;
            ViewBag.SelectedGenre = genre;
            ViewBag.Genres = genres;

            return View(paginatedSeries);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Accessing TV series details for ID: {SeriesId}", id);

            var seriesDto = await _tvSeriesService.GetSeriesByIdAsync(id);
            if (seriesDto == null)
            {
                _logger.LogWarning("TV series not found: {SeriesId}", id);
                return NotFound();
            }

            var series = _mapper.Map<TVSeries>(seriesDto);

            var reviewsDto = await _reviewService.GetReviewsByMovieIdAsync(id);
            var reviews = _mapper.Map<IEnumerable<Review>>(reviewsDto);
            var averageRating = await _reviewService.GetAverageRatingAsync(id);
            var reviewCount = await _reviewService.GetReviewCountAsync(id);
            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(1, id); // userId = 1 for demo

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.IsInWatchlist = isInWatchlist;

            // Get similar series based on genre (exclude current series)
            var allSeriesDto = await _tvSeriesService.GetAllSeriesAsync();
            var allSeries = _mapper.Map<IEnumerable<TVSeries>>(allSeriesDto);
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
                var createReviewDto = new CreateReviewDto
                {
                    MovieId = seriesId, // Using MovieId field for SeriesId
                    UserId = 1,
                    Comment = comment,
                    Rating = rating
                };

                await _reviewService.AddReviewAsync(createReviewDto);
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
