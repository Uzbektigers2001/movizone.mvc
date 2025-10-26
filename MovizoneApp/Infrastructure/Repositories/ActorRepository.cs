using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Models;

namespace MovizoneApp.Infrastructure.Repositories
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        public ActorRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Override to include MovieActors, TVSeriesActors, and their related entities
        public override async Task<Actor?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(a => a.MovieActors)
                    .ThenInclude(ma => ma.Movie)
                .Include(a => a.TVSeriesActors)
                    .ThenInclude(tsa => tsa.TVSeries)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public override async Task<System.Collections.Generic.IEnumerable<Actor>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.MovieActors)
                    .ThenInclude(ma => ma.Movie)
                .Include(a => a.TVSeriesActors)
                    .ThenInclude(tsa => tsa.TVSeries)
                .ToListAsync();
        }

        public async Task<Actor?> GetActorWithDetailsAsync(int id)
        {
            return await GetByIdAsync(id); // Use the overridden method
        }
    }
}
