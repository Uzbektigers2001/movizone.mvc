using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Models;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieService _movieService;
    private readonly ITVSeriesService _tvSeriesService;

    public HomeController(ILogger<HomeController> logger, IMovieService movieService, ITVSeriesService tvSeriesService)
    {
        _logger = logger;
        _movieService = movieService;
        _tvSeriesService = tvSeriesService;
    }

    public IActionResult Index(int page = 1)
    {
        // Banner Movies (only movies with ShowInBanner=true and not hidden)
        var bannerMovies = _movieService.GetAllMovies()
            .Where(m => m.ShowInBanner && !m.IsHidden)
            .OrderByDescending(m => m.Rating)
            .ToList();

        // Banner Series (only series with ShowInBanner=true and not hidden)
        var bannerSeries = _tvSeriesService.GetAllSeries()
            .Where(s => s.ShowInBanner && !s.IsHidden)
            .OrderByDescending(s => s.Rating)
            .ToList();

        // Pagination for catalog grid
        int pageSize = 18;
        var allFeaturedMovies = _movieService.GetFeaturedMovies().Where(m => !m.IsHidden).ToList();
        var totalPages = (int)Math.Ceiling(allFeaturedMovies.Count / (double)pageSize);
        var featuredMovies = allFeaturedMovies
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        var featuredSeries = _tvSeriesService.GetFeaturedSeries().Where(s => !s.IsHidden).ToList();

        // Top Rated (rating >= 8.0)
        var topRatedMovies = _movieService.GetAllMovies()
            .Where(m => !m.IsHidden)
            .OrderByDescending(m => m.Rating)
            .Take(6)
            .ToList();

        // Recent Additions (last 30 days or most recent)
        var recentMovies = _movieService.GetAllMovies()
            .Where(m => !m.IsHidden)
            .OrderByDescending(m => m.ReleaseDate)
            .Take(6)
            .ToList();

        // Popular TV Series
        var popularSeries = _tvSeriesService.GetAllSeries()
            .Where(s => !s.IsHidden)
            .OrderByDescending(s => s.Rating)
            .Take(6)
            .ToList();

        ViewBag.BannerMovies = bannerMovies;
        ViewBag.BannerSeries = bannerSeries;
        ViewBag.FeaturedMovies = featuredMovies;
        ViewBag.FeaturedSeries = featuredSeries;
        ViewBag.TopRatedMovies = topRatedMovies;
        ViewBag.RecentMovies = recentMovies;
        ViewBag.PopularSeries = popularSeries;

        // Genres for filter
        ViewBag.Genres = _movieService.GetAllMovies().Where(m => !m.IsHidden).Select(m => m.Genre).Distinct().ToList();

        // Statistics
        ViewBag.TotalMovies = _movieService.GetAllMovies().Where(m => !m.IsHidden).Count();
        ViewBag.TotalSeries = _tvSeriesService.GetAllSeries().Where(s => !s.IsHidden).Count();

        // Recently Updated - ordered by release date/first aired
        var recentlyUpdatedMovies = _movieService.GetAllMovies()
            .Where(m => !m.IsHidden)
            .OrderByDescending(m => m.ReleaseDate)
            .Take(12)
            .ToList();

        var recentlyUpdatedSeries = _tvSeriesService.GetAllSeries()
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
