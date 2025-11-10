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
    public class MovieController : BaseController
    {
        private readonly IMovieApplicationService _movieService;
        private readonly IReviewApplicationService _reviewService;
        private readonly ILogger<MovieController> _logger;
        private readonly IMapper _mapper;

        public MovieController(
            IMovieApplicationService movieService,
            IReviewApplicationService reviewService,
            ILogger<MovieController> logger,
            IMapper mapper)
        {
            _movieService = movieService;
            _reviewService = reviewService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Catalog(string search = "", string genre = "", int page = 1)
        {
            _logger.LogInformation("Accessing movie catalog. Search: {Search}, Genre: {Genre}, Page: {Page}", search, genre, page);

            var allMoviesDto = await _movieService.SearchMoviesAsync(search, genre);
            var genres = await _movieService.GetAllGenresAsync();

            // Create paginated list
            const int pageSize = 12;
            var paginatedMovies = PaginatedList<MovieDto>.Create(allMoviesDto, page, pageSize);

            ViewBag.SearchQuery = search;
            ViewBag.SelectedGenre = genre;
            ViewBag.Genres = genres;

            return View(paginatedMovies);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Accessing movie details for ID: {MovieId}", id);

            var movieDto = await _movieService.GetMovieByIdAsync(id);
            if (movieDto == null)
            {
                _logger.LogWarning("Movie not found: {MovieId}", id);
                return NotFound();
            }

            var reviewsDto = await _reviewService.GetReviewsByMovieIdAsync(id);
            var reviews = _mapper.Map<IEnumerable<Review>>(reviewsDto);
            var averageRating = await _reviewService.GetAverageRatingAsync(id);
            var reviewCount = await _reviewService.GetReviewCountAsync(id);

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;

            // Get similar movies based on genre (exclude current movie) - optimized query
            var similarMovies = await _movieService.GetSimilarMoviesByGenreAsync(id, movieDto.Genre, 6);
            ViewBag.SimilarMovies = similarMovies;

            return View(movieDto);
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
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    TempData["Error"] = "You must be logged in to add a review";
                    return RedirectToAction("Details", new { id = movieId });
                }

                var createReviewDto = new CreateReviewDto
                {
                    MovieId = movieId,
                    UserId = userId.Value,
                    Comment = comment,
                    Rating = rating
                };

                await _reviewService.AddReviewAsync(createReviewDto);
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
