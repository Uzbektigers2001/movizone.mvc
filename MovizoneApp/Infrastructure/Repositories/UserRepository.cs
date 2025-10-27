using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Models;

namespace MovizoneApp.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            // Use PostgreSQL ILike for case-insensitive search (optimized for indexes)
            return await _dbSet.FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email));
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            // Use PostgreSQL ILike for case-insensitive search (optimized for indexes)
            return await _dbSet.AnyAsync(u => EF.Functions.ILike(u.Email, email));
        }
    }
}
