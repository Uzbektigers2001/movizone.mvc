using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly IWatchlistApplicationService _watchlistService;
        private readonly IMovieApplicationService _movieService;
        private readonly ITVSeriesApplicationService _tvSeriesService;
        private readonly ILogger<WatchlistController> _logger;
        private readonly IMapper _mapper;

        public WatchlistController(
            IWatchlistApplicationService watchlistService,
            IMovieApplicationService movieService,
            ITVSeriesApplicationService tvSeriesService,
            ILogger<WatchlistController> logger,
            IMapper mapper)
        {
            _watchlistService = watchlistService;
            _movieService = movieService;
            _tvSeriesService = tvSeriesService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _logger.LogInformation("Fetching watchlist for user {UserId}", userId);

            var watchlistDtos = await _watchlistService.GetUserWatchlistAsync(userId);

            // Fetch movie details for each watchlist item
            var movies = new List<object>();
            foreach (var itemDto in watchlistDtos)
            {
                var movieDto = await _movieService.GetMovieByIdAsync(itemDto.MovieId);
                if (movieDto != null)
                {
                    movies.Add(new
                    {
                        WatchlistId = itemDto.Id,
                        MovieId = movieDto.Id,
                        Title = movieDto.Title,
                        CoverImage = movieDto.CoverImage,
                        Rating = movieDto.Rating,
                        Year = movieDto.Year,
                        Genre = movieDto.Genre,
                        AddedAt = itemDto.AddedAt,
                        Type = "Movie"
                    });
                }
                else
                {
                    var seriesDto = await _tvSeriesService.GetSeriesByIdAsync(itemDto.MovieId);
                    if (seriesDto != null)
                    {
                        movies.Add(new
                        {
                            WatchlistId = itemDto.Id,
                            MovieId = seriesDto.Id,
                            Title = seriesDto.Title,
                            CoverImage = seriesDto.CoverImage,
                            Rating = seriesDto.Rating,
                            Year = seriesDto.Year,
                            Genre = seriesDto.Genre,
                            AddedAt = itemDto.AddedAt,
                            Type = "Series"
                        });
                    }
                }
            }

            ViewBag.WatchlistItems = movies;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(int movieId, string returnUrl)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _logger.LogInformation("Adding movie {MovieId} to watchlist for user {UserId}", movieId, userId);

            try
            {
                var createDto = new CreateWatchlistItemDto
                {
                    UserId = userId,
                    MovieId = movieId
                };
                await _watchlistService.AddToWatchlistAsync(createDto);
                TempData["Success"] = "Added to your watchlist!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to watchlist for user {UserId}", userId);
                TempData["Error"] = "Failed to add to watchlist.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int movieId, string returnUrl)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            _logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", movieId, userId);

            try
            {
                await _watchlistService.RemoveFromWatchlistAsync(userId, movieId);
                TempData["Success"] = "Removed from your watchlist!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from watchlist for user {UserId}", userId);
                TempData["Error"] = "Failed to remove from watchlist.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> IsInWatchlist(int movieId)
        {
            // In real app, get userId from authenticated user
            int userId = 1;
            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(userId, movieId);
            return Json(new { isInWatchlist });
        }
    }
}
