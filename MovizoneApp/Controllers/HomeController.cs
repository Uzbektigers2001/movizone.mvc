using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieApplicationService _movieService;
    private readonly ITVSeriesApplicationService _tvSeriesService;
    private readonly IMapper _mapper;

    public HomeController(
        ILogger<HomeController> logger,
        IMovieApplicationService movieService,
        ITVSeriesApplicationService tvSeriesService,
        IMapper mapper)
    {
        _logger = logger;
        _movieService = movieService;
        _tvSeriesService = tvSeriesService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(
        string genre = "",
        string ratingFrom = "",
        string yearFrom = "",
        string sort = "newest",
        int page = 1)
    {
        _logger.LogInformation("Accessing home page - Genre: {Genre}, Page: {Page}", genre, page);

        try
        {
            // Get all movies and series async
            var allMoviesDto = await _movieService.GetAllMoviesAsync();
            var allMovies = _mapper.Map<IEnumerable<Movie>>(allMoviesDto);
            var allSeriesDto = await _tvSeriesService.GetAllSeriesAsync();
            var allSeries = _mapper.Map<IEnumerable<TVSeries>>(allSeriesDto);

            // Banner items (movies and series with ShowInBanner=true and not hidden)
            var bannerMovies = allMovies
                .Where(m => m.ShowInBanner && !m.IsHidden)
                .OrderByDescending(m => m.Rating)
                .Take(5)
                .ToList();

            var bannerSeries = allSeries
                .Where(s => s.ShowInBanner && !s.IsHidden)
                .OrderByDescending(s => s.Rating)
                .Take(5)
                .ToList();

            // Pagination for catalog grid with filters
            int pageSize = 18;
            var filteredMovies = allMovies.Where(m => !m.IsHidden).ToList();

            // Apply filters
            if (!string.IsNullOrEmpty(genre))
            {
                filteredMovies = filteredMovies.Where(m => m.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(ratingFrom) && double.TryParse(ratingFrom, out double minRating))
            {
                filteredMovies = filteredMovies.Where(m => m.Rating >= minRating).ToList();
            }

            if (!string.IsNullOrEmpty(yearFrom) && int.TryParse(yearFrom, out int year))
            {
                if (year == 2020)
                {
                    filteredMovies = filteredMovies.Where(m => m.Year >= 2020).ToList();
                }
                else
                {
                    filteredMovies = filteredMovies.Where(m => m.Year == year).ToList();
                }
            }

            // Apply sorting
            filteredMovies = sort switch
            {
                "rating" => filteredMovies.OrderByDescending(m => m.Rating).ToList(),
                "title" => filteredMovies.OrderBy(m => m.Title).ToList(),
                _ => filteredMovies.OrderByDescending(m => m.ReleaseDate).ToList() // newest
            };

            var totalPages = (int)Math.Ceiling(filteredMovies.Count / (double)pageSize);
            var paginatedMovies = filteredMovies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SelectedGenre = genre;
            ViewBag.SelectedRatingFrom = ratingFrom;
            ViewBag.SelectedYearFrom = yearFrom;
            ViewBag.SelectedSort = sort;

            // Featured content
            var featuredMovies = allMovies.Where(m => m.IsFeatured && !m.IsHidden).Take(6).ToList();
            var featuredSeries = allSeries.Where(s => s.IsFeatured && !s.IsHidden).Take(6).ToList();

            // Top Rated (rating >= 8.0)
            var topRatedMovies = allMovies
                .Where(m => !m.IsHidden)
                .OrderByDescending(m => m.Rating)
                .Take(6)
                .ToList();

            // Recent Additions (most recent by release date)
            var recentMovies = allMovies
                .Where(m => !m.IsHidden)
                .OrderByDescending(m => m.ReleaseDate)
                .Take(6)
                .ToList();

            // Popular TV Series
            var popularSeries = allSeries
                .Where(s => !s.IsHidden)
                .OrderByDescending(s => s.Rating)
                .Take(6)
                .ToList();

            ViewBag.BannerMovies = bannerMovies;
            ViewBag.BannerSeries = bannerSeries;
            ViewBag.FeaturedMovies = featuredMovies;
            ViewBag.FeaturedSeries = featuredSeries;
            ViewBag.PaginatedMovies = paginatedMovies;
            ViewBag.TopRatedMovies = topRatedMovies;
            ViewBag.RecentMovies = recentMovies;
            ViewBag.PopularSeries = popularSeries;

            // Genres for filter
            var genres = await _movieService.GetAllGenresAsync();
            ViewBag.Genres = genres.Where(g => !string.IsNullOrEmpty(g)).Distinct().ToList();

            // Statistics
            ViewBag.TotalMovies = allMovies.Count(m => !m.IsHidden);
            ViewBag.TotalSeries = allSeries.Count(s => !s.IsHidden);

            // Recently Updated - ordered by release date/first aired
            var recentlyUpdatedMovies = allMovies
                .Where(m => !m.IsHidden)
                .OrderByDescending(m => m.ReleaseDate)
                .Take(12)
                .ToList();

            var recentlyUpdatedSeries = allSeries
                .Where(s => !s.IsHidden)
                .OrderByDescending(s => s.FirstAired)
                .Take(12)
                .ToList();

            // Combine for "New items" tab
            var recentlyUpdatedAll = new List<(string Title, double Rating, string Genre, string CoverImage, string Type, int Id, DateTime Date)>();

            foreach (var movie in recentlyUpdatedMovies)
            {
                recentlyUpdatedAll.Add((movie.Title, movie.Rating, movie.Genre, movie.CoverImage, "movie", movie.Id, movie.ReleaseDate));
            }
            foreach (var series in recentlyUpdatedSeries)
            {
                recentlyUpdatedAll.Add((series.Title, series.Rating, series.Genre, series.CoverImage, "series", series.Id, series.FirstAired));
            }

            // Sort combined list by date
            recentlyUpdatedAll = recentlyUpdatedAll.OrderByDescending(x => x.Date).Take(12).ToList();

            ViewBag.RecentlyUpdatedAll = recentlyUpdatedAll;
            ViewBag.RecentlyUpdatedMovies = recentlyUpdatedMovies;
            ViewBag.RecentlyUpdatedSeries = recentlyUpdatedSeries;

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contacts()
    {
        return View();
    }

    public IActionResult Faq()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
