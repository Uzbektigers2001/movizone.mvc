using System.Collections.Generic;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IReviewService
    {
        List<Review> GetAllReviews();
        List<Review> GetReviewsByMovieId(int movieId);
        List<Review> GetReviewsByUserId(int userId);
        void AddReview(Review review);
        void DeleteReview(int id);
    }
}
