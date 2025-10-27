using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Models;

namespace MovizoneApp.Infrastructure.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Override to include MovieActors and Actor navigation properties
        public override async Task<Movie?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetFeaturedMoviesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Where(m => m.IsFeatured)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string? searchTerm, string? genre)
        {
            var query = _dbSet
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Use PostgreSQL ILike for case-insensitive search (optimized for indexes)
                var pattern = $"%{searchTerm}%";
                query = query.Where(m =>
                    EF.Functions.ILike(m.Title, pattern) ||
                    EF.Functions.ILike(m.Description, pattern) ||
                    EF.Functions.ILike(m.Genre, pattern));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(m => m.Genre == genre);
            }

            return await query.OrderByDescending(m => m.Rating).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctGenresAsync()
        {
            // Use database-level distinct instead of fetching all movies
            return await _dbSet
                .AsNoTracking()
                .Select(m => m.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetSimilarMoviesByGenreAsync(int movieId, string genre, int take = 6)
        {
            // Use database-level filtering instead of fetching all movies
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Where(m => m.Id != movieId && m.Genre == genre)
                .OrderByDescending(m => m.Rating)
                .Take(take)
                .ToListAsync();
        }
    }
}
