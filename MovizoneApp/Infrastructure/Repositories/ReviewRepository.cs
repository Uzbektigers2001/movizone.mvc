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
                .AsNoTracking()
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int movieId)
        {
            // Use database-level aggregation instead of fetching all reviews
            var reviews = _dbSet.Where(r => r.MovieId == movieId);

            if (!await reviews.AnyAsync())
                return 0;

            return await reviews.AverageAsync(r => r.Rating);
        }

        public async Task<IEnumerable<Review>> GetReviewsByTVSeriesIdAsync(int tvSeriesId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(r => r.TVSeriesId == tvSeriesId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingByTVSeriesIdAsync(int tvSeriesId)
        {
            // Use database-level aggregation instead of fetching all reviews
            var reviews = _dbSet.Where(r => r.TVSeriesId == tvSeriesId);

            if (!await reviews.AnyAsync())
                return 0;

            return await reviews.AverageAsync(r => r.Rating);
        }
    }
}
