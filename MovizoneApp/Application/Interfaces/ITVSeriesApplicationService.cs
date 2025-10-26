using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for TV series-related business logic
    /// Now uses DTOs instead of exposing database models
    /// </summary>
    public interface ITVSeriesApplicationService
    {
        Task<IEnumerable<TVSeriesDto>> GetAllSeriesAsync();
        Task<IEnumerable<TVSeriesDto>> GetFeaturedSeriesAsync();
        Task<IEnumerable<TVSeriesDto>> SearchSeriesAsync(string? searchTerm, string? genre);
        Task<TVSeriesDto?> GetSeriesByIdAsync(int id);
        Task<TVSeriesDto> CreateSeriesAsync(CreateTVSeriesDto createSeriesDto);
        Task UpdateSeriesAsync(UpdateTVSeriesDto updateSeriesDto);
        Task DeleteSeriesAsync(int id);
        Task<IEnumerable<string>> GetAllGenresAsync();
        Task<bool> ExistsAsync(int id);
    }
}
