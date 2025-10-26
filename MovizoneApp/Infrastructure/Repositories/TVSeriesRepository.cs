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
                .Include(s => s.TVSeriesActors)
                    .ThenInclude(tsa => tsa.Actor)
                .ToListAsync();
        }

        public async Task<IEnumerable<TVSeries>> GetFeaturedSeriesAsync()
        {
            return await _dbSet
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
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(s =>
                    s.Title.ToLower().Contains(lowerSearchTerm) ||
                    s.Description.ToLower().Contains(lowerSearchTerm) ||
                    s.Genre.ToLower().Contains(lowerSearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(s => s.Genre == genre);
            }

            return await query.OrderByDescending(s => s.Rating).ToListAsync();
        }
    }
}
