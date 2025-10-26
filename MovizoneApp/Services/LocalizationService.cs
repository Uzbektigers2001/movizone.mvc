using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MovizoneApp.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        private Dictionary<string, Dictionary<string, object>> _localizations = new();
        private const string CookieName = "UserLanguage";
        private const string DefaultCulture = "en";

        private readonly List<(string Code, string Name)> _availableLanguages = new()
        {
            ("en", "English"),
            ("uz", "O'zbek"),
            ("ru", "Русский")
        };

        public LocalizationService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
            LoadLocalizations();
        }

        private void LoadLocalizations()
        {
            var resourcesPath = Path.Combine(_environment.ContentRootPath, "Resources");

            foreach (var (code, _) in _availableLanguages)
            {
                var filePath = Path.Combine(resourcesPath, $"localization.{code}.json");
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (data != null)
                    {
                        _localizations[code] = data;
                    }
                }
            }
        }

        public string GetCurrentCulture()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return DefaultCulture;

            // Try to get from cookie
            if (httpContext.Request.Cookies.TryGetValue(CookieName, out var culture))
            {
                if (_availableLanguages.Any(l => l.Code == culture))
                    return culture;
            }

            // Try to get from session
            var sessionCulture = httpContext.Session.GetString("Culture");
            if (!string.IsNullOrEmpty(sessionCulture) && _availableLanguages.Any(l => l.Code == sessionCulture))
                return sessionCulture;

            return DefaultCulture;
        }

        public void SetCulture(string culture)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !_availableLanguages.Any(l => l.Code == culture))
                return;

            // Set cookie (expires in 1 year)
            httpContext.Response.Cookies.Append(CookieName, culture, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });

            // Also set in session for immediate access
            httpContext.Session.SetString("Culture", culture);
        }

        public string GetString(string key)
        {
            return GetString("Common", key);
        }

        public string GetString(string section, string key)
        {
            var culture = GetCurrentCulture();

            if (!_localizations.ContainsKey(culture))
                return $"[{section}.{key}]";

            try
            {
                var localization = _localizations[culture];

                if (localization.TryGetValue(section, out var sectionObj))
                {
                    if (sectionObj is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
                    {
                        if (jsonElement.TryGetProperty(key, out var keyElement))
                        {
                            return keyElement.GetString() ?? $"[{section}.{key}]";
                        }
                    }
                }
            }
            catch
            {
                // Return key if there's any error
            }

            return $"[{section}.{key}]";
        }

        public List<(string Code, string Name)> GetAvailableLanguages()
        {
            return _availableLanguages;
        }
    }
}
