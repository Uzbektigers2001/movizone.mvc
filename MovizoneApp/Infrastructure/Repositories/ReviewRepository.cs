using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Models;

namespace MovizoneApp.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(int movieId)
        {
            return await _dbSet
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int movieId)
        {
            var reviews = await _dbSet.Where(r => r.MovieId == movieId).ToListAsync();
            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<IEnumerable<Review>> GetReviewsByTVSeriesIdAsync(int tvSeriesId)
        {
            return await _dbSet
                .Where(r => r.TVSeriesId == tvSeriesId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingByTVSeriesIdAsync(int tvSeriesId)
        {
            var reviews = await _dbSet.Where(r => r.TVSeriesId == tvSeriesId).ToListAsync();
            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}
