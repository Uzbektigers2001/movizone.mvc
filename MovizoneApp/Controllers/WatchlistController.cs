using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly IWatchlistService _watchlistService;
        private readonly IMovieService _movieService;
        private readonly ITVSeriesService _tvSeriesService;

        public WatchlistController(
            IWatchlistService watchlistService,
            IMovieService movieService,
            ITVSeriesService tvSeriesService)
        {
            _watchlistService = watchlistService;
            _movieService = movieService;
            _tvSeriesService = tvSeriesService;
        }

        public IActionResult Index()
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            var watchlistItems = _watchlistService.GetUserWatchlist(userId);

            // Fetch movie details for each watchlist item
            var movies = new List<object>();
            foreach (var item in watchlistItems)
            {
                var movie = _movieService.GetMovieById(item.MovieId);
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
                    var series = _tvSeriesService.GetSeriesById(item.MovieId);
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
        public IActionResult Add(int movieId, string returnUrl)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _watchlistService.AddToWatchlist(userId, movieId);
            TempData["Success"] = "Added to your watchlist!";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int movieId, string returnUrl)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _watchlistService.RemoveFromWatchlist(userId, movieId);
            TempData["Success"] = "Removed from your watchlist!";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult IsInWatchlist(int movieId)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            var isInWatchlist = _watchlistService.IsInWatchlist(userId, movieId);
            return Json(new { isInWatchlist });
        }
    }
}
