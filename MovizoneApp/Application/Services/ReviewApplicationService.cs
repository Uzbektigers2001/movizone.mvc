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
    /// Application service for review business logic
    /// </summary>
    public class ReviewApplicationService : IReviewApplicationService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ITVSeriesRepository _tvSeriesRepository;
        private readonly ILogger<ReviewApplicationService> _logger;

        public ReviewApplicationService(
            IReviewRepository reviewRepository,
            IMovieRepository movieRepository,
            ITVSeriesRepository tvSeriesRepository,
            ILogger<ReviewApplicationService> logger)
        {
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
            _tvSeriesRepository = tvSeriesRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(int movieId)
        {
            _logger.LogInformation("Fetching reviews for movie ID: {MovieId}", movieId);
            return await _reviewRepository.GetReviewsByMovieIdAsync(movieId);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            _logger.LogInformation("Adding review for {ContentType} ID: {ContentId}",
                review.ContentType, review.ContentId);

            // Business validation
            if (string.IsNullOrWhiteSpace(review.Comment))
            {
                throw new BadRequestException("Review comment is required");
            }

            if (string.IsNullOrWhiteSpace(review.UserName))
            {
                throw new BadRequestException("User name is required");
            }

            if (review.Rating < 1 || review.Rating > 10)
            {
                throw new BadRequestException("Rating must be between 1 and 10");
            }

            // Validate that either MovieId or TVSeriesId is set
            if (!review.MovieId.HasValue && !review.TVSeriesId.HasValue)
            {
                throw new BadRequestException("Review must be associated with either a Movie or TV Series");
            }

            if (review.MovieId.HasValue && review.TVSeriesId.HasValue)
            {
                throw new BadRequestException("Review cannot be associated with both a Movie and TV Series");
            }

            // Check if content exists
            if (review.MovieId.HasValue)
            {
                var movieExists = await _movieRepository.ExistsAsync(m => m.Id == review.MovieId.Value);
                if (!movieExists)
                {
                    throw new NotFoundException("Movie", review.MovieId.Value);
                }
            }
            else if (review.TVSeriesId.HasValue)
            {
                var seriesExists = await _tvSeriesRepository.ExistsAsync(s => s.Id == review.TVSeriesId.Value);
                if (!seriesExists)
                {
                    throw new NotFoundException("TV Series", review.TVSeriesId.Value);
                }
            }

            review.CreatedAt = DateTime.UtcNow;
            var created = await _reviewRepository.AddAsync(review);

            _logger.LogInformation("Review added successfully with ID: {ReviewId}", created.Id);
            return created;
        }

        public async Task DeleteReviewAsync(int id)
        {
            _logger.LogInformation("Deleting review with ID: {ReviewId}", id);

            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                throw new NotFoundException("Review", id);
            }

            await _reviewRepository.DeleteAsync(review);
            _logger.LogInformation("Review deleted successfully: {ReviewId}", id);
        }

        public async Task<double> GetAverageRatingAsync(int movieId)
        {
            _logger.LogInformation("Calculating average rating for movie ID: {MovieId}", movieId);
            return await _reviewRepository.GetAverageRatingAsync(movieId);
        }

        public async Task<int> GetReviewCountAsync(int movieId)
        {
            _logger.LogInformation("Counting reviews for movie ID: {MovieId}", movieId);
            var reviews = await _reviewRepository.GetReviewsByMovieIdAsync(movieId);
            return reviews.Count();
        }

        public async Task<IEnumerable<Review>> GetReviewsByTVSeriesIdAsync(int tvSeriesId)
        {
            _logger.LogInformation("Fetching reviews for TV series ID: {TVSeriesId}", tvSeriesId);
            return await _reviewRepository.GetReviewsByTVSeriesIdAsync(tvSeriesId);
        }

        public async Task<double> GetAverageRatingForTVSeriesAsync(int tvSeriesId)
        {
            _logger.LogInformation("Calculating average rating for TV series ID: {TVSeriesId}", tvSeriesId);
            return await _reviewRepository.GetAverageRatingForTVSeriesAsync(tvSeriesId);
        }

        public async Task<int> GetReviewCountForTVSeriesAsync(int tvSeriesId)
        {
            _logger.LogInformation("Counting reviews for TV series ID: {TVSeriesId}", tvSeriesId);
            var reviews = await _reviewRepository.GetReviewsByTVSeriesIdAsync(tvSeriesId);
            return reviews.Count();
        }
    }
}
