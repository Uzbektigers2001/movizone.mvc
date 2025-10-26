using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface ISiteSettingsService
    {
        SiteSettings GetSettings();
        void UpdateSettings(SiteSettings settings);
    }
}
