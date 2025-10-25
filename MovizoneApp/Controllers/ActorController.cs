using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class ActorController : Controller
    {
        private readonly IActorService _actorService;
        private readonly IMovieService _movieService;
        private readonly ITVSeriesService _tvSeriesService;

        public ActorController(IActorService actorService, IMovieService movieService, ITVSeriesService tvSeriesService)
        {
            _actorService = actorService;
            _movieService = movieService;
            _tvSeriesService = tvSeriesService;
        }

        public IActionResult Index()
        {
            var actors = _actorService.GetAllActors();
            return View(actors);
        }

        public IActionResult Details(int id)
        {
            var actor = _actorService.GetActorById(id);
            if (actor == null)
            {
                return NotFound();
            }

            // Get movies featuring this actor
            var movies = _movieService.GetAllMovies()
                .Where(m => m.Actors.Contains(actor.Name))
                .ToList();

            // Get TV series featuring this actor
            var series = _tvSeriesService.GetAllSeries()
                .Where(s => s.Actors.Contains(actor.Name))
                .ToList();

            ViewBag.Movies = movies;
            ViewBag.TVSeries = series;

            return View(actor);
        }
    }
}
