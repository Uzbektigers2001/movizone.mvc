using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for pricing plan-related business logic
    /// Now uses DTOs instead of exposing database models
    /// </summary>
    public interface IPricingApplicationService
    {
        Task<IEnumerable<PricingPlanDto>> GetAllPlansAsync();
        Task<PricingPlanDto?> GetPlanByIdAsync(int id);
        Task<PricingPlanDto?> GetPopularPlanAsync();
        Task<PricingPlanDto> CreatePlanAsync(CreatePricingPlanDto createPlanDto);
        Task UpdatePlanAsync(UpdatePricingPlanDto updatePlanDto);
        Task DeletePlanAsync(int id);
    }
}
