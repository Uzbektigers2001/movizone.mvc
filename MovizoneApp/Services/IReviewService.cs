using System.Collections.Generic;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IReviewService
    {
        List<Review> GetReviewsByMovieId(int movieId);
        void AddReview(Review review);
        void DeleteReview(int id);
    }
}
