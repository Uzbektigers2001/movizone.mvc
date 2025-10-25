using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class TVSeriesController : Controller
    {
        private readonly ITVSeriesService _tvSeriesService;

        public TVSeriesController(ITVSeriesService tvSeriesService)
        {
            _tvSeriesService = tvSeriesService;
        }

        public IActionResult Catalog()
        {
            var series = _tvSeriesService.GetAllSeries();
            return View(series);
        }

        public IActionResult Details(int id)
        {
            var series = _tvSeriesService.GetSeriesById(id);
            if (series == null)
            {
                return NotFound();
            }
            return View(series);
        }
    }
}
