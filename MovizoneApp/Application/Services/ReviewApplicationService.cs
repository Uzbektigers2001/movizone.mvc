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
    /// Application service for review business logic
    /// Uses DTOs and AutoMapper for clean separation from database models
    /// </summary>
    public class ReviewApplicationService : IReviewApplicationService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewApplicationService> _logger;

        public ReviewApplicationService(
            IReviewRepository reviewRepository,
            IMovieRepository movieRepository,
            IMapper mapper,
            ILogger<ReviewApplicationService> logger)
        {
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByMovieIdAsync(int movieId)
        {
            _logger.LogInformation("Fetching reviews for movie ID: {MovieId}", movieId);
            var reviews = await _reviewRepository.GetReviewsByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto)
        {
            _logger.LogInformation("Adding review for movie ID: {MovieId}", createReviewDto.MovieId);

            // Map DTO to Model
            var review = _mapper.Map<Review>(createReviewDto);

            // Business validation (additional to DTO validation)
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

            // Set timestamps
            review.CreatedAt = DateTime.UtcNow;

            // Save to repository
            var created = await _reviewRepository.AddAsync(review);

            _logger.LogInformation("Review added successfully with ID: {ReviewId}", created.Id);

            // Map back to DTO and return
            return _mapper.Map<ReviewDto>(created);
        }

        public async Task UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            try
            {
                _logger.LogInformation("Updating review with ID: {ReviewId}", updateReviewDto.Id);

                // Check if review exists
                var existing = await _reviewRepository.GetByIdAsync(updateReviewDto.Id);
                if (existing == null)
                {
                    throw new NotFoundException("Review", updateReviewDto.Id);
                }

                // Map DTO properties to existing tracked entity
                _mapper.Map(updateReviewDto, existing);

                // Business validation
                if (string.IsNullOrWhiteSpace(existing.Comment))
                {
                    throw new BadRequestException("Review comment is required");
                }

                if (existing.Rating < 1 || existing.Rating > 10)
                {
                    throw new BadRequestException("Rating must be between 1 and 10");
                }

                // Set update time (CreatedAt already preserved in existing entity)
                existing.UpdatedAt = DateTime.UtcNow;

                // Update in repository
                await _reviewRepository.UpdateAsync(existing);
                _logger.LogInformation("Review updated successfully: {ReviewId}", existing.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review with ID: {ReviewId}", updateReviewDto.Id);
                throw;
            }
        }

        public async Task DeleteReviewAsync(int id)
        {
            _logger.LogInformation("Deleting review with ID: {ReviewId}", id);

            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                throw new NotFoundException("Review", id);
            }

            // Soft delete
            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;
            // TODO: Set DeletedBy from current user context when authentication is available
            // entity.DeletedBy = currentUserId;
            review.UpdatedAt = DateTime.UtcNow;
            await _reviewRepository.UpdateAsync(review);

            _logger.LogInformation("Review soft-deleted successfully: {ReviewId}", id);
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
