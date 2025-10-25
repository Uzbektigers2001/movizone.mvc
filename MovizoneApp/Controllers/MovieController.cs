using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Catalog(string search = "")
        {
            var movies = string.IsNullOrEmpty(search)
                ? _movieService.GetAllMovies()
                : _movieService.SearchMovies(search);

            ViewBag.SearchQuery = search;
            return View(movies);
        }

        public IActionResult Details(int id)
        {
            var movie = _movieService.GetMovieById(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
    }
}
