using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieApplicationService _movieService;
        private readonly IReviewApplicationService _reviewService;
        private readonly IWatchlistApplicationService _watchlistService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(
            IMovieApplicationService movieService,
            IReviewApplicationService reviewService,
            IWatchlistApplicationService watchlistService,
            ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _reviewService = reviewService;
            _watchlistService = watchlistService;
            _logger = logger;
        }

        public async Task<IActionResult> Catalog(string search = "", string genre = "")
        {
            _logger.LogInformation("Accessing movie catalog. Search: {Search}, Genre: {Genre}", search, genre);

            var movies = await _movieService.SearchMoviesAsync(search, genre);
            var genres = await _movieService.GetAllGenresAsync();

            ViewBag.SearchQuery = search;
            ViewBag.SelectedGenre = genre;
            ViewBag.Genres = genres;

            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Accessing movie details for ID: {MovieId}", id);

            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie not found: {MovieId}", id);
                return NotFound();
            }

            var reviews = await _reviewService.GetReviewsByMovieIdAsync(id);
            var averageRating = await _reviewService.GetAverageRatingAsync(id);
            var reviewCount = await _reviewService.GetReviewCountAsync(id);
            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(1, id); // userId = 1 for demo

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.IsInWatchlist = isInWatchlist;

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int movieId, string userName, string comment, int rating)
        {
            _logger.LogInformation("Adding review for movie ID: {MovieId}", movieId);

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(comment))
            {
                TempData["Error"] = "Please fill in all fields";
                return RedirectToAction("Details", new { id = movieId });
            }

            try
            {
                var review = new Review
                {
                    MovieId = movieId,
                    UserId = 1, // In real app, get from authenticated user
                    UserName = userName,
                    Comment = comment,
                    Rating = rating
                };

                await _reviewService.AddReviewAsync(review);
                TempData["Success"] = "Review added successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review for movie {MovieId}", movieId);
                TempData["Error"] = "Failed to add review. Please try again.";
            }

            return RedirectToAction("Details", new { id = movieId });
        }
    }
}
