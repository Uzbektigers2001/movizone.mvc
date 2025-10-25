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

    public IActionResult Index()
    {
        var featuredMovies = _movieService.GetFeaturedMovies();
        var featuredSeries = _tvSeriesService.GetFeaturedSeries();

        // Top Rated (rating >= 8.0)
        var topRatedMovies = _movieService.GetAllMovies()
            .OrderByDescending(m => m.Rating)
            .Take(6)
            .ToList();

        // Recent Additions (last 30 days or most recent)
        var recentMovies = _movieService.GetAllMovies()
            .OrderByDescending(m => m.ReleaseDate)
            .Take(6)
            .ToList();

        // Popular TV Series
        var popularSeries = _tvSeriesService.GetAllSeries()
            .OrderByDescending(s => s.Rating)
            .Take(6)
            .ToList();

        ViewBag.FeaturedMovies = featuredMovies;
        ViewBag.FeaturedSeries = featuredSeries;
        ViewBag.TopRatedMovies = topRatedMovies;
        ViewBag.RecentMovies = recentMovies;
        ViewBag.PopularSeries = popularSeries;

        // Genres for filter
        ViewBag.Genres = _movieService.GetAllMovies().Select(m => m.Genre).Distinct().ToList();

        // Statistics
        ViewBag.TotalMovies = _movieService.GetAllMovies().Count;
        ViewBag.TotalSeries = _tvSeriesService.GetAllSeries().Count;

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
