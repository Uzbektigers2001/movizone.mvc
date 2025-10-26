using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for watchlist business logic
    /// </summary>
    public class WatchlistApplicationService : IWatchlistApplicationService
    {
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<WatchlistApplicationService> _logger;

        public WatchlistApplicationService(
            IWatchlistRepository watchlistRepository,
            IMovieRepository movieRepository,
            IUserRepository userRepository,
            ILogger<WatchlistApplicationService> logger)
        {
            _watchlistRepository = watchlistRepository;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<WatchlistItem>> GetUserWatchlistAsync(int userId)
        {
            _logger.LogInformation("Fetching watchlist for user ID: {UserId}", userId);
            return await _watchlistRepository.GetUserWatchlistAsync(userId);
        }

        public async Task<IEnumerable<Movie>> GetUserWatchlistWithMoviesAsync(int userId)
        {
            _logger.LogInformation("Fetching watchlist with movies for user ID: {UserId}", userId);

            var watchlistItems = await _watchlistRepository.GetUserWatchlistAsync(userId);
            var movieIds = watchlistItems.Select(w => w.MovieId).ToList();

            if (!movieIds.Any())
            {
                return new List<Movie>();
            }

            var movies = await _movieRepository.FindAsync(m => movieIds.Contains(m.Id));
            return movies;
        }

        public async Task<WatchlistItem> AddToWatchlistAsync(int userId, int movieId)
        {
            _logger.LogInformation("Adding movie {MovieId} to watchlist for user {UserId}", movieId, userId);

            // Business validation
            var userExists = await _userRepository.ExistsAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new NotFoundException("User", userId);
            }

            var movieExists = await _movieRepository.ExistsAsync(m => m.Id == movieId);
            if (!movieExists)
            {
                throw new NotFoundException("Movie", movieId);
            }

            // Check if already in watchlist
            if (await _watchlistRepository.IsInWatchlistAsync(userId, movieId))
            {
                throw new BadRequestException("Movie is already in watchlist");
            }

            var watchlistItem = new WatchlistItem
            {
                UserId = userId,
                MovieId = movieId,
                AddedAt = DateTime.UtcNow
            };

            var created = await _watchlistRepository.AddAsync(watchlistItem);
            _logger.LogInformation("Movie added to watchlist successfully");

            return created;
        }

        public async Task RemoveFromWatchlistAsync(int userId, int movieId)
        {
            _logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", movieId, userId);

            var item = await _watchlistRepository.FirstOrDefaultAsync(w =>
                w.UserId == userId && w.MovieId == movieId);

            if (item == null)
            {
                throw new NotFoundException("Watchlist item not found");
            }

            await _watchlistRepository.DeleteAsync(item);
            _logger.LogInformation("Movie removed from watchlist successfully");
        }

        public async Task<bool> IsInWatchlistAsync(int userId, int movieId)
        {
            return await _watchlistRepository.IsInWatchlistAsync(userId, movieId);
        }
    }
}
