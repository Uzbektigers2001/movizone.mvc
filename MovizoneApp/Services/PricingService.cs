using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class PricingService : IPricingService
    {
        private readonly List<PricingPlan> _plans;

        public PricingService()
        {
            _plans = new List<PricingPlan>
            {
                new PricingPlan
                {
                    Id = 1,
                    Name = "Basic",
                    Price = 9.99m,
                    Period = "month",
                    Features = new List<string>
                    {
                        "HD Available",
                        "Watch on TV or laptop",
                        "Unlimited movies and TV shows",
                        "Cancel anytime"
                    },
                    IsPopular = false
                },
                new PricingPlan
                {
                    Id = 2,
                    Name = "Premium",
                    Price = 19.99m,
                    Period = "month",
                    Features = new List<string>
                    {
                        "Ultra HD Available",
                        "Watch on TV, laptop, tablet, and phone",
                        "Unlimited movies and TV shows",
                        "Download on 4 devices",
                        "Cancel anytime"
                    },
                    IsPopular = true
                },
                new PricingPlan
                {
                    Id = 3,
                    Name = "Cinematic",
                    Price = 39.99m,
                    Period = "month",
                    Features = new List<string>
                    {
                        "Ultra HD + 4K",
                        "Watch on any device",
                        "Unlimited movies and TV shows",
                        "Download on unlimited devices",
                        "Priority support",
                        "Cancel anytime"
                    },
                    IsPopular = false
                }
            };
        }

        public List<PricingPlan> GetAllPlans() => _plans;
    }
}
