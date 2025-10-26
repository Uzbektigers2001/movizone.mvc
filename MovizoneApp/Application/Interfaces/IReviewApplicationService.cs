using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for review-related business logic
    /// Now uses DTOs instead of exposing database models
    /// </summary>
    public interface IReviewApplicationService
    {
        // Movie reviews
        Task<IEnumerable<ReviewDto>> GetReviewsByMovieIdAsync(int movieId);
        Task<double> GetAverageRatingAsync(int movieId);
        Task<int> GetReviewCountAsync(int movieId);

        // TVSeries reviews
        Task<IEnumerable<ReviewDto>> GetReviewsByTVSeriesIdAsync(int tvSeriesId);
        Task<double> GetAverageRatingByTVSeriesIdAsync(int tvSeriesId);
        Task<int> GetReviewCountByTVSeriesIdAsync(int tvSeriesId);

        // Common review operations (works for both Movie and TVSeries)
        Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto);
        Task UpdateReviewAsync(UpdateReviewDto updateReviewDto);
        Task DeleteReviewAsync(int id);
    }
}
