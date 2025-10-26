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
        Task<IEnumerable<ReviewDto>> GetReviewsByMovieIdAsync(int movieId);
        Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto);
        Task UpdateReviewAsync(UpdateReviewDto updateReviewDto);
        Task DeleteReviewAsync(int id);
        Task<double> GetAverageRatingAsync(int movieId);
        Task<int> GetReviewCountAsync(int movieId);
    }
}
