using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ITVSeriesService _tvSeriesService;
        private readonly IActorService _actorService;

        public SearchController(IMovieService movieService, ITVSeriesService tvSeriesService, IActorService actorService)
        {
            _movieService = movieService;
            _tvSeriesService = tvSeriesService;
            _actorService = actorService;
        }

        public IActionResult Index(
            string query = "",
            string type = "all",
            string genre = "",
            int? yearFrom = null,
            int? yearTo = null,
            double? ratingFrom = null,
            double? ratingTo = null,
            string actor = "")
        {
            var movies = new List<Movie>();
            var series = new List<TVSeries>();

            if (type == "all" || type == "movies")
            {
                movies = _movieService.GetAllMovies();

                // Apply filters
                if (!string.IsNullOrEmpty(query))
                {
                    movies = movies.Where(m =>
                        m.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        m.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        m.Director.Contains(query, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    movies = movies.Where(m => m.Genre == genre).ToList();
                }

                if (yearFrom.HasValue)
                {
                    movies = movies.Where(m => m.Year >= yearFrom.Value).ToList();
                }

                if (yearTo.HasValue)
                {
                    movies = movies.Where(m => m.Year <= yearTo.Value).ToList();
                }

                if (ratingFrom.HasValue)
                {
                    movies = movies.Where(m => m.Rating >= ratingFrom.Value).ToList();
                }

                if (ratingTo.HasValue)
                {
                    movies = movies.Where(m => m.Rating <= ratingTo.Value).ToList();
                }

                if (!string.IsNullOrEmpty(actor))
                {
                    movies = movies.Where(m => m.Actors.Any(a => a.Contains(actor, StringComparison.OrdinalIgnoreCase))).ToList();
                }
            }

            if (type == "all" || type == "series")
            {
                series = _tvSeriesService.GetAllSeries();

                // Apply filters
                if (!string.IsNullOrEmpty(query))
                {
                    series = series.Where(s =>
                        s.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        s.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        s.Creator.Contains(query, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    series = series.Where(s => s.Genre == genre).ToList();
                }

                if (yearFrom.HasValue)
                {
                    series = series.Where(s => s.Year >= yearFrom.Value).ToList();
                }

                if (yearTo.HasValue)
                {
                    series = series.Where(s => s.Year <= yearTo.Value).ToList();
                }

                if (ratingFrom.HasValue)
                {
                    series = series.Where(s => s.Rating >= ratingFrom.Value).ToList();
                }

                if (ratingTo.HasValue)
                {
                    series = series.Where(s => s.Rating <= ratingTo.Value).ToList();
                }

                if (!string.IsNullOrEmpty(actor))
                {
                    series = series.Where(s => s.Actors.Any(a => a.Contains(actor, StringComparison.OrdinalIgnoreCase))).ToList();
                }
            }

            ViewBag.Query = query;
            ViewBag.Type = type;
            ViewBag.Genre = genre;
            ViewBag.YearFrom = yearFrom;
            ViewBag.YearTo = yearTo;
            ViewBag.RatingFrom = ratingFrom;
            ViewBag.RatingTo = ratingTo;
            ViewBag.Actor = actor;
            ViewBag.Movies = movies;
            ViewBag.Series = series;
            ViewBag.TotalResults = movies.Count + series.Count;

            // Get all genres for dropdown
            var allGenres = _movieService.GetAllMovies().Select(m => m.Genre)
                .Concat(_tvSeriesService.GetAllSeries().Select(s => s.Genre))
                .Distinct()
                .OrderBy(g => g)
                .ToList();
            ViewBag.Genres = allGenres;

            return View();
        }
    }
}
