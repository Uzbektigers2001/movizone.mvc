using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.DTOs;

namespace MovizoneApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly IMovieApplicationService _movieService;
        private readonly ITVSeriesApplicationService _tvSeriesService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            IMovieApplicationService movieService,
            ITVSeriesApplicationService tvSeriesService,
            ILogger<SearchController> logger)
        {
            _movieService = movieService;
            _tvSeriesService = tvSeriesService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string query = "",
            string type = "all",
            string genre = "",
            int? yearFrom = null,
            int? yearTo = null,
            double? ratingFrom = null,
            double? ratingTo = null,
            string actor = "")
        {
            _logger.LogInformation("Search request - Query: {Query}, Type: {Type}, Genre: {Genre}", query, type, genre);

            var movieDtos = new List<MovieDto>();
            var seriesDtos = new List<TVSeriesDto>();

            if (type == "all" || type == "movies")
            {
                var allMoviesDto = await _movieService.GetAllMoviesAsync();
                movieDtos = allMoviesDto.ToList();

                // Apply filters
                if (!string.IsNullOrEmpty(query))
                {
                    movieDtos = movieDtos.Where(m =>
                        m.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        m.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        m.Director.Contains(query, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    movieDtos = movieDtos.Where(m => m.Genre == genre).ToList();
                }

                if (yearFrom.HasValue)
                {
                    movieDtos = movieDtos.Where(m => m.Year >= yearFrom.Value).ToList();
                }

                if (yearTo.HasValue)
                {
                    movieDtos = movieDtos.Where(m => m.Year <= yearTo.Value).ToList();
                }

                if (ratingFrom.HasValue)
                {
                    movieDtos = movieDtos.Where(m => m.Rating >= ratingFrom.Value).ToList();
                }

                if (ratingTo.HasValue)
                {
                    movieDtos = movieDtos.Where(m => m.Rating <= ratingTo.Value).ToList();
                }

                if (!string.IsNullOrEmpty(actor))
                {
                    movieDtos = movieDtos.Where(m => m.Actors.Any(a => a.Contains(actor, StringComparison.OrdinalIgnoreCase))).ToList();
                }
            }

            if (type == "all" || type == "series")
            {
                var allSeriesDto = await _tvSeriesService.GetAllSeriesAsync();
                seriesDtos = allSeriesDto.ToList();

                // Apply filters
                if (!string.IsNullOrEmpty(query))
                {
                    seriesDtos = seriesDtos.Where(s =>
                        s.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        s.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        s.Director.Contains(query, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    seriesDtos = seriesDtos.Where(s => s.Genre == genre).ToList();
                }

                if (yearFrom.HasValue)
                {
                    seriesDtos = seriesDtos.Where(s => s.Year >= yearFrom.Value).ToList();
                }

                if (yearTo.HasValue)
                {
                    seriesDtos = seriesDtos.Where(s => s.Year <= yearTo.Value).ToList();
                }

                if (ratingFrom.HasValue)
                {
                    seriesDtos = seriesDtos.Where(s => s.Rating >= ratingFrom.Value).ToList();
                }

                if (ratingTo.HasValue)
                {
                    seriesDtos = seriesDtos.Where(s => s.Rating <= ratingTo.Value).ToList();
                }

                if (!string.IsNullOrEmpty(actor))
                {
                    seriesDtos = seriesDtos.Where(s => s.Actors.Any(a => a.Contains(actor, StringComparison.OrdinalIgnoreCase))).ToList();
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
            ViewBag.Movies = movieDtos;
            ViewBag.Series = seriesDtos;
            ViewBag.TotalResults = movieDtos.Count + seriesDtos.Count;

            // Get all genres for dropdown
            var movieGenres = await _movieService.GetAllGenresAsync();
            var seriesGenres = await _tvSeriesService.GetAllGenresAsync();
            var allGenres = movieGenres.Concat(seriesGenres)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
            ViewBag.Genres = allGenres;

            _logger.LogInformation("Search completed - Found {MovieCount} movies and {SeriesCount} series", movieDtos.Count, seriesDtos.Count);
            return View();
        }
    }
}
