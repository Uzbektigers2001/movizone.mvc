using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for movie-related business logic
    /// Now uses DTOs instead of exposing database models
    /// </summary>
    public interface IMovieApplicationService
    {
        Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
        Task<IEnumerable<MovieDto>> GetFeaturedMoviesAsync();
        Task<IEnumerable<MovieDto>> SearchMoviesAsync(string? searchTerm, string? genre);
        Task<MovieDto?> GetMovieByIdAsync(int id);
        Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto);
        Task UpdateMovieAsync(UpdateMovieDto updateMovieDto);
        Task DeleteMovieAsync(int id);
        Task<IEnumerable<string>> GetAllGenresAsync();
        Task<bool> ExistsAsync(int id);
    }
}
