using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class SiteSettings : BaseAuditableEntity
    {
        // Banner/Hero Section
        public bool ShowHeroBanner { get; set; } = true;

        // Home Page Sections
        public bool ShowCatalogSection { get; set; } = true;
        public bool ShowExpectedPremiere { get; set; } = true;
        public bool ShowRecentlyUpdated { get; set; } = true;
        public bool ShowPricingPlans { get; set; } = true;

        // Navigation Menu Items
        public bool ShowHomeInNav { get; set; } = true;
        public bool ShowMoviesInNav { get; set; } = true;
        public bool ShowTVSeriesInNav { get; set; } = true;
        public bool ShowActorsInNav { get; set; } = true;
        public bool ShowPricingPlanInNav { get; set; } = true;
        public bool ShowPagesInNav { get; set; } = true;
    }
}
