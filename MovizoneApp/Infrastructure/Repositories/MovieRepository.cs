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
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetFeaturedMoviesAsync()
        {
            return await _dbSet
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
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(m =>
                    m.Title.ToLower().Contains(lowerSearchTerm) ||
                    m.Description.ToLower().Contains(lowerSearchTerm) ||
                    m.Genre.ToLower().Contains(lowerSearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(m => m.Genre == genre);
            }

            return await query.OrderByDescending(m => m.Rating).ToListAsync();
        }
    }
}
