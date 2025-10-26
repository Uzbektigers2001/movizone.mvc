using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;

namespace MovizoneApp.Controllers
{
    public class PricingController : Controller
    {
        private readonly IPricingApplicationService _pricingService;
        private readonly ILogger<PricingController> _logger;

        public PricingController(
            IPricingApplicationService pricingService,
            ILogger<PricingController> logger)
        {
            _pricingService = pricingService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching pricing plans");

            var plans = await _pricingService.GetAllPlansAsync();
            return View(plans);
        }
    }
}
