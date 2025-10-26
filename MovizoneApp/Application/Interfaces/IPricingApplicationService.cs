using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for pricing plan-related business logic
    /// </summary>
    public interface IPricingApplicationService
    {
        Task<IEnumerable<PricingPlan>> GetAllPlansAsync();
        Task<PricingPlan?> GetPlanByIdAsync(int id);
        Task<PricingPlan?> GetPopularPlanAsync();
    }
}
