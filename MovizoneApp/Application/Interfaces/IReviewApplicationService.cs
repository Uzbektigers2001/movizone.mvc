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
        Task<Review> AddReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
        Task<double> GetAverageRatingAsync(int movieId);
        Task<int> GetReviewCountAsync(int movieId);
    }
}
