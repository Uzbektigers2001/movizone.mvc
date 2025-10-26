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
        private readonly ILogger<ReviewApplicationService> _logger;

        public ReviewApplicationService(
            IReviewRepository reviewRepository,
            IMovieRepository movieRepository,
            ILogger<ReviewApplicationService> logger)
        {
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(int movieId)
        {
            _logger.LogInformation("Fetching reviews for movie ID: {MovieId}", movieId);
            return await _reviewRepository.GetReviewsByMovieIdAsync(movieId);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            _logger.LogInformation("Adding review for movie ID: {MovieId}", review.MovieId);

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

            // Check if movie exists
            var movieExists = await _movieRepository.ExistsAsync(m => m.Id == review.MovieId);
            if (!movieExists)
            {
                throw new NotFoundException("Movie", review.MovieId);
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
    }
}
