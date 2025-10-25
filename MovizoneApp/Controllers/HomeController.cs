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
        var featuredMovies = _movieService.GetFeaturedMovies();
        var featuredSeries = _tvSeriesService.GetFeaturedSeries();

        ViewBag.FeaturedMovies = featuredMovies;
        ViewBag.FeaturedSeries = featuredSeries;

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
