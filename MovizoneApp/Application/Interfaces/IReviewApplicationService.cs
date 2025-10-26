using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for review-related business logic
    /// </summary>
    public interface IReviewApplicationService
    {
        Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(int movieId);
        Task<IEnumerable<Review>> GetReviewsByTVSeriesIdAsync(int tvSeriesId);
        Task<Review> AddReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
        Task<double> GetAverageRatingAsync(int movieId);
        Task<double> GetAverageRatingForTVSeriesAsync(int tvSeriesId);
        Task<int> GetReviewCountAsync(int movieId);
        Task<int> GetReviewCountForTVSeriesAsync(int tvSeriesId);
    }
}
