using Microsoft.AspNetCore.Mvc;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class ActorController : Controller
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        public IActionResult Details(int id)
        {
            var actor = _actorService.GetActorById(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }
    }
}
