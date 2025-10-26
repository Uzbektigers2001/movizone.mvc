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

        public async Task<IEnumerable<Movie>> GetFeaturedMoviesAsync()
        {
            return await _dbSet.Where(m => m.IsFeatured).ToListAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string? searchTerm, string? genre)
        {
            var query = _dbSet.AsQueryable();

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
