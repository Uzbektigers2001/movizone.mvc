using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IWatchlistService
    {
        List<WatchlistItem> GetUserWatchlist(int userId);
        void AddToWatchlist(int userId, int movieId);
        void RemoveFromWatchlist(int userId, int movieId);
        bool IsInWatchlist(int userId, int movieId);
    }
}
