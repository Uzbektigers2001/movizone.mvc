using System;
using System.Collections.Generic;
using System.Linq;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly List<Review> _reviews;

        public ReviewService()
        {
            _reviews = new List<Review>
            {
                new Review
                {
                    Id = 1,
                    MovieId = 1,
                    UserId = 2,
                    UserName = "John Doe",
                    Comment = "Amazing movie! Great visual effects and storyline.",
                    Rating = 9,
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Review
                {
                    Id = 2,
                    MovieId = 1,
                    UserId = 3,
                    UserName = "Jane Smith",
                    Comment = "Really enjoyed it. Tom Cruise was excellent!",
                    Rating = 8,
                    CreatedAt = DateTime.Now.AddDays(-3)
                }
            };
        }

        public List<Review> GetReviewsByMovieId(int movieId)
        {
            return _reviews.Where(r => r.MovieId == movieId).OrderByDescending(r => r.CreatedAt).ToList();
        }

        public void AddReview(Review review)
        {
            review.Id = _reviews.Any() ? _reviews.Max(r => r.Id) + 1 : 1;
            review.CreatedAt = DateTime.Now;
            _reviews.Add(review);
        }

        public void DeleteReview(int id)
        {
            var review = _reviews.FirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                _reviews.Remove(review);
            }
        }
    }
}
