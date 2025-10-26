using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;

namespace MovizoneApp.Controllers
{
    public class ActorController : Controller
    {
        private readonly IActorApplicationService _actorService;
        private readonly ILogger<ActorController> _logger;

        public ActorController(
            IActorApplicationService actorService,
            ILogger<ActorController> logger)
        {
            _actorService = actorService;
            _logger = logger;
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Fetching actor details for ID: {ActorId}", id);

            var actor = await _actorService.GetActorWithDetailsAsync(id);
            if (actor == null)
            {
                _logger.LogWarning("Actor not found: {ActorId}", id);
                return NotFound();
            }

            return View(actor);
        }
    }
}
