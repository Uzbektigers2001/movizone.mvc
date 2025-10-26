using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for watchlist-related business logic
    /// </summary>
    public interface IWatchlistApplicationService
    {
        Task<IEnumerable<WatchlistItem>> GetUserWatchlistAsync(int userId);
        Task<IEnumerable<Movie>> GetUserWatchlistWithMoviesAsync(int userId);
        Task<WatchlistItem> AddToWatchlistAsync(int userId, int movieId);
        Task RemoveFromWatchlistAsync(int userId, int movieId);
        Task<bool> IsInWatchlistAsync(int userId, int movieId);
    }
}
