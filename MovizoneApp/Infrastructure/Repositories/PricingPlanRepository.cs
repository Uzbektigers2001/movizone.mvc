using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Models;

namespace MovizoneApp.Infrastructure.Repositories
{
    public class PricingPlanRepository : Repository<PricingPlan>, IPricingPlanRepository
    {
        public PricingPlanRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
