using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class ActorController : Controller
    {
        private readonly IActorApplicationService _actorService;
        private readonly ILogger<ActorController> _logger;
        private readonly IMapper _mapper;

        public ActorController(
            IActorApplicationService actorService,
            ILogger<ActorController> logger,
            IMapper mapper)
        {
            _actorService = actorService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all actors");

            var actorsDto = await _actorService.GetAllActorsAsync();
            var actors = _mapper.Map<IEnumerable<Actor>>(actorsDto);
            return View(actors.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Fetching actor details for ID: {ActorId}", id);

            var actorDto = await _actorService.GetActorWithDetailsAsync(id);
            if (actorDto == null)
            {
                _logger.LogWarning("Actor not found: {ActorId}", id);
                return NotFound();
            }

            var actor = _mapper.Map<Actor>(actorDto);
            return View(actor);
        }
    }
}
