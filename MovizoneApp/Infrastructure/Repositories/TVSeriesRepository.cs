using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Models;

namespace MovizoneApp.Infrastructure.Repositories
{
    public class TVSeriesRepository : Repository<TVSeries>, ITVSeriesRepository
    {
        public TVSeriesRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Override to include TVSeriesActors and Actor navigation properties
        public override async Task<TVSeries?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.TVSeriesActors)
                    .ThenInclude(tsa => tsa.Actor)
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public override async Task<IEnumerable<TVSeries>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.TVSeriesActors)
                    .ThenInclude(tsa => tsa.Actor)
                .ToListAsync();
        }

        public async Task<IEnumerable<TVSeries>> GetFeaturedSeriesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.TVSeriesActors)
                    .ThenInclude(tsa => tsa.Actor)
                .Where(s => s.IsFeatured)
                .ToListAsync();
        }

        public async Task<IEnumerable<TVSeries>> SearchSeriesAsync(string? searchTerm, string? genre)
        {
            var query = _dbSet
                .Include(s => s.TVSeriesActors)
                    .ThenInclude(tsa => tsa.Actor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Use PostgreSQL ILike for case-insensitive search (optimized for indexes)
                var pattern = $"%{searchTerm}%";
                query = query.Where(s =>
                    EF.Functions.ILike(s.Title, pattern) ||
                    EF.Functions.ILike(s.Description, pattern) ||
                    EF.Functions.ILike(s.Genre, pattern));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(s => s.Genre == genre);
            }

            return await query.OrderByDescending(s => s.Rating).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctGenresAsync()
        {
            // Use database-level distinct instead of fetching all series
            return await _dbSet
                .AsNoTracking()
                .Select(s => s.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();
        }
    }
}
