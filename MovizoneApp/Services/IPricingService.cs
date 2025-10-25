using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IPricingService
    {
        List<PricingPlan> GetAllPlans();
    }
}
