using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

    public IActionResult Index()
    {
        // Banner Movies (only movies with ShowInBanner=true and not hidden)
        var bannerMovies = _movieService.GetAllMovies()
            .Where(m => m.ShowInBanner && !m.IsHidden)
            .OrderByDescending(m => m.Rating)
            .Take(5)
            .ToList();

        var featuredMovies = _movieService.GetFeaturedMovies().Where(m => !m.IsHidden).ToList();
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
