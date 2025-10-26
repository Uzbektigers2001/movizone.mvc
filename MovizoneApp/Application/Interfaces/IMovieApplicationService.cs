using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for movie-related business logic
    /// </summary>
    public interface IMovieApplicationService
    {
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<IEnumerable<Movie>> GetFeaturedMoviesAsync();
        Task<IEnumerable<Movie>> SearchMoviesAsync(string? searchTerm, string? genre);
        Task<Movie?> GetMovieByIdAsync(int id);
        Task<Movie> CreateMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(int id);
        Task<IEnumerable<string>> GetAllGenresAsync();
        Task<bool> ExistsAsync(int id);
    }
}
