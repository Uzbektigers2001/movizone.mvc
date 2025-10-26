using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private static SiteSettings _settings = new SiteSettings();

        public SiteSettings GetSettings()
        {
            return _settings;
        }

        public void UpdateSettings(SiteSettings settings)
        {
            _settings = settings;
        }
    }
}
