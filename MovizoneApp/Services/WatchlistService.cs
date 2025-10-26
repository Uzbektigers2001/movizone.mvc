using System;
using System.Collections.Generic;
using System.Linq;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly List<WatchlistItem> _watchlist;

        public WatchlistService()
        {
            _watchlist = new List<WatchlistItem>();
        }

        public List<WatchlistItem> GetUserWatchlist(int userId)
        {
            return _watchlist.Where(w => w.UserId == userId).OrderByDescending(w => w.CreatedAt).ToList();
        }

        public void AddToWatchlist(int userId, int movieId)
        {
            if (!IsInWatchlist(userId, movieId))
            {
                _watchlist.Add(new WatchlistItem
                {
                    Id = _watchlist.Any() ? _watchlist.Max(w => w.Id) + 1 : 1,
                    UserId = userId,
                    MovieId = movieId,
                    CreatedAt = DateTime.Now
                });
            }
        }

        public void RemoveFromWatchlist(int userId, int movieId)
        {
            var item = _watchlist.FirstOrDefault(w => w.UserId == userId && w.MovieId == movieId);
            if (item != null)
            {
                _watchlist.Remove(item);
            }
        }

        public bool IsInWatchlist(int userId, int movieId)
        {
            return _watchlist.Any(w => w.UserId == userId && w.MovieId == movieId);
        }
    }
}
