using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieApplicationService _movieService;
    private readonly ITVSeriesApplicationService _tvSeriesService;

    public HomeController(
        ILogger<HomeController> logger,
        IMovieApplicationService movieService,
        ITVSeriesApplicationService tvSeriesService)
    {
        _logger = logger;
        _movieService = movieService;
        _tvSeriesService = tvSeriesService;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Accessing home page");

        var featuredMovies = await _movieService.GetFeaturedMoviesAsync();
        var featuredSeries = await _tvSeriesService.GetFeaturedSeriesAsync();

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
