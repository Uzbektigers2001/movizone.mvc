using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Models;
using MovizoneApp.Services;
using MovizoneApp.Helpers;

namespace MovizoneApp.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IReviewService _reviewService;
        private readonly IWatchlistService _watchlistService;

        public MovieController(IMovieService movieService, IReviewService reviewService, IWatchlistService watchlistService)
        {
            _movieService = movieService;
            _reviewService = reviewService;
            _watchlistService = watchlistService;
        }

        public IActionResult Catalog(string search = "", string genre = "", int pageNumber = 1)
        {
            var movies = _movieService.GetAllMovies();

            if (!string.IsNullOrEmpty(search))
            {
                movies = _movieService.SearchMovies(search);
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                movies = movies.Where(m => m.Genre == genre).ToList();
            }

            ViewBag.SearchQuery = search;
            ViewBag.SelectedGenre = genre;
            ViewBag.Genres = _movieService.GetAllMovies().Select(m => m.Genre).Distinct().OrderBy(g => g).ToList();

            int pageSize = 12;
            var paginatedMovies = PaginatedList<Movie>.Create(movies, pageNumber, pageSize);

            return View(paginatedMovies);
        }

        public IActionResult Details(int id)
        {
            var movie = _movieService.GetMovieById(id);
            if (movie == null)
            {
                return NotFound();
            }

            var reviews = _reviewService.GetReviewsByMovieId(id);
            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            ViewBag.ReviewCount = reviews.Count;

            // Check if in watchlist (userId = 1 for demo)
            ViewBag.IsInWatchlist = _watchlistService.IsInWatchlist(1, id);

            return View(movie);
        }

        [HttpPost]
        public IActionResult AddReview(int movieId, string userName, string comment, int rating)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(comment))
            {
                TempData["Error"] = "Please fill in all fields";
                return RedirectToAction("Details", new { id = movieId });
            }

            var review = new Review
            {
                MovieId = movieId,
                UserId = 1, // In real app, get from authenticated user
                UserName = userName,
                Comment = comment,
                Rating = rating
            };

            _reviewService.AddReview(review);
            TempData["Success"] = "Review added successfully!";
            return RedirectToAction("Details", new { id = movieId });
        }
    }
}
