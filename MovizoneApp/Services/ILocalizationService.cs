namespace MovizoneApp.Services
{
    public interface ILocalizationService
    {
        string GetString(string key);
        string GetString(string section, string key);
        void SetCulture(string culture);
        string GetCurrentCulture();
        List<(string Code, string Name)> GetAvailableLanguages();
    }
}
