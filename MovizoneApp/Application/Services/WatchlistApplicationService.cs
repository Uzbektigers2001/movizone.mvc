using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for watchlist business logic
    /// Uses DTOs and AutoMapper for clean separation from database models
    /// </summary>
    public class WatchlistApplicationService : IWatchlistApplicationService
    {
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<WatchlistApplicationService> _logger;

        public WatchlistApplicationService(
            IWatchlistRepository watchlistRepository,
            IMovieRepository movieRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<WatchlistApplicationService> logger)
        {
            _watchlistRepository = watchlistRepository;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<WatchlistDto>> GetUserWatchlistAsync(int userId)
        {
            _logger.LogInformation("Fetching watchlist for user ID: {UserId}", userId);
            var watchlistItems = await _watchlistRepository.GetUserWatchlistAsync(userId);
            return _mapper.Map<IEnumerable<WatchlistDto>>(watchlistItems);
        }

        public async Task<WatchlistDto> AddToWatchlistAsync(CreateWatchlistItemDto createWatchlistItemDto)
        {
            _logger.LogInformation("Adding movie {MovieId} to watchlist for user {UserId}",
                createWatchlistItemDto.MovieId, createWatchlistItemDto.UserId);

            // Map DTO to Model
            var watchlistItem = _mapper.Map<WatchlistItem>(createWatchlistItemDto);

            // Business validation (additional to DTO validation)
            var userExists = await _userRepository.ExistsAsync(u => u.Id == watchlistItem.UserId);
            if (!userExists)
            {
                throw new NotFoundException("User", watchlistItem.UserId);
            }

            var movieExists = await _movieRepository.ExistsAsync(m => m.Id == watchlistItem.MovieId);
            if (!movieExists)
            {
                throw new NotFoundException("Movie", watchlistItem.MovieId);
            }

            // Check if already in watchlist
            if (await _watchlistRepository.IsInWatchlistAsync(watchlistItem.UserId, watchlistItem.MovieId))
            {
                throw new BadRequestException("Movie is already in watchlist");
            }

            // Set timestamp
            watchlistItem.AddedAt = DateTime.UtcNow;

            // Save to repository
            var created = await _watchlistRepository.AddAsync(watchlistItem);
            _logger.LogInformation("Movie added to watchlist successfully");

            // Map back to DTO and return
            return _mapper.Map<WatchlistDto>(created);
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
