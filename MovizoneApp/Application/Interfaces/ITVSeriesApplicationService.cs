using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for TV series-related business logic
    /// </summary>
    public interface ITVSeriesApplicationService
    {
        Task<IEnumerable<TVSeries>> GetAllSeriesAsync();
        Task<IEnumerable<TVSeries>> GetFeaturedSeriesAsync();
        Task<IEnumerable<TVSeries>> SearchSeriesAsync(string? searchTerm, string? genre);
        Task<TVSeries?> GetSeriesByIdAsync(int id);
        Task<TVSeries> CreateSeriesAsync(TVSeries series);
        Task UpdateSeriesAsync(TVSeries series);
        Task DeleteSeriesAsync(int id);
        Task<IEnumerable<string>> GetAllGenresAsync();
        Task<bool> ExistsAsync(int id);
    }
}
