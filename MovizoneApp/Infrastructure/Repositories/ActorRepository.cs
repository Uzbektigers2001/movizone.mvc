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

        public async Task<Actor?> GetActorWithDetailsAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
