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
    public class WatchlistController : BaseController
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
            return await GetWatchlistView();
        }

        // Favorites is same as Watchlist
        public async Task<IActionResult> Favorites()
        {
            return await GetWatchlistView("Favorites");
        }

        private async Task<IActionResult> GetWatchlistView(string viewName = "Index")
        {
            // Check if user is logged in
            var userIdSession = HttpContext.Session.GetInt32("UserId");

            if (userIdSession.HasValue)
            {
                // User is logged in - fetch from database
                int userId = userIdSession.Value;
                _logger.LogInformation("Fetching watchlist for logged-in user {UserId}", userId);

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
                            CreatedAt = itemDto.CreatedAt,
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
                                CreatedAt = itemDto.CreatedAt,
                                Type = "Series"
                            });
                        }
                    }
                }

                ViewBag.WatchlistItems = movies;
                ViewBag.IsLoggedIn = true;
            }
            else
            {
                // User is not logged in - view will load from localStorage via JavaScript
                _logger.LogInformation("Anonymous user accessing watchlist - will load from localStorage");
                ViewBag.WatchlistItems = new List<object>();
                ViewBag.IsLoggedIn = false;
            }

            return View(viewName);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int movieId, string returnUrl)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "You must be logged in to add to watchlist";
                return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index") : Redirect(returnUrl);
            }

            _logger.LogInformation("Adding movie {MovieId} to watchlist for user {UserId}", movieId, userId.Value);

            try
            {
                var createDto = new CreateWatchlistItemDto
                {
                    UserId = userId.Value,
                    MovieId = movieId
                };
                await _watchlistService.AddToWatchlistAsync(createDto);
                TempData["Success"] = "Added to your watchlist!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to watchlist for user {UserId}", userId.Value);
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
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "You must be logged in to remove from watchlist";
                return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index") : Redirect(returnUrl);
            }

            _logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", movieId, userId.Value);

            try
            {
                await _watchlistService.RemoveFromWatchlistAsync(userId.Value, movieId);
                TempData["Success"] = "Removed from your watchlist!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from watchlist for user {UserId}", userId.Value);
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
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Json(new { isInWatchlist = false });
            }

            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(userId.Value, movieId);
            return Json(new { isInWatchlist });
        }
    }
}
