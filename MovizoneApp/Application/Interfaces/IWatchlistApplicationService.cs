using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for watchlist-related business logic
    /// Now uses DTOs instead of exposing database models
    /// </summary>
    public interface IWatchlistApplicationService
    {
        Task<IEnumerable<WatchlistDto>> GetUserWatchlistAsync(int userId);
        Task<WatchlistDto> AddToWatchlistAsync(CreateWatchlistItemDto createWatchlistItemDto);
        Task RemoveFromWatchlistAsync(int userId, int movieId);
        Task<bool> IsInWatchlistAsync(int userId, int movieId);
    }
}
