using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IReviewService
    {
        List<Review> GetReviewsByMovieId(int movieId);
        List<Review> GetReviewsByTVSeriesId(int tvSeriesId);
        List<Review> GetReviewsByUserId(int userId);
        void AddReview(Review review);
        void DeleteReview(int id);
    }
}
