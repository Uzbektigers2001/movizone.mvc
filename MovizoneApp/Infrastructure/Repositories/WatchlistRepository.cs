using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Models;

namespace MovizoneApp.Infrastructure.Repositories
{
    public class WatchlistRepository : Repository<WatchlistItem>, IWatchlistRepository
    {
        public WatchlistRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WatchlistItem>> GetUserWatchlistAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsInWatchlistAsync(int userId, int movieId)
        {
            return await _dbSet.AnyAsync(w => w.UserId == userId && w.MovieId == movieId);
        }
    }
}
