using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class PricingController : Controller
    {
        private readonly IPricingApplicationService _pricingService;
        private readonly ILogger<PricingController> _logger;
        private readonly IMapper _mapper;

        public PricingController(
            IPricingApplicationService pricingService,
            ILogger<PricingController> logger,
            IMapper mapper)
        {
            _pricingService = pricingService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching pricing plans");

            var plansDto = await _pricingService.GetAllPlansAsync();
            var plans = _mapper.Map<IEnumerable<PricingPlan>>(plansDto);
            return View(plans.ToList());
        }
    }
}
