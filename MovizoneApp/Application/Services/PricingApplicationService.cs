using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for pricing plan business logic
    /// </summary>
    public class PricingApplicationService : IPricingApplicationService
    {
        private readonly IPricingPlanRepository _pricingRepository;
        private readonly ILogger<PricingApplicationService> _logger;

        public PricingApplicationService(
            IPricingPlanRepository pricingRepository,
            ILogger<PricingApplicationService> logger)
        {
            _pricingRepository = pricingRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<PricingPlan>> GetAllPlansAsync()
        {
            _logger.LogInformation("Fetching all pricing plans");
            return await _pricingRepository.GetAllAsync();
        }

        public async Task<PricingPlan?> GetPlanByIdAsync(int id)
        {
            _logger.LogInformation("Fetching pricing plan with ID: {PlanId}", id);
            return await _pricingRepository.GetByIdAsync(id);
        }

        public async Task<PricingPlan?> GetPopularPlanAsync()
        {
            _logger.LogInformation("Fetching popular pricing plan");
            var plans = await _pricingRepository.GetAllAsync();
            return plans.FirstOrDefault(p => p.IsPopular);
        }
    }
}
