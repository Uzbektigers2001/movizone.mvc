using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface for common CRUD operations
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }

    public interface IMovieRepository : IRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetFeaturedMoviesAsync();
        Task<IEnumerable<Movie>> SearchMoviesAsync(string? searchTerm, string? genre);
    }

    public interface ITVSeriesRepository : IRepository<TVSeries>
    {
        Task<IEnumerable<TVSeries>> GetFeaturedSeriesAsync();
        Task<IEnumerable<TVSeries>> SearchSeriesAsync(string? searchTerm, string? genre);
    }

    public interface IActorRepository : IRepository<Actor>
    {
        Task<Actor?> GetActorWithDetailsAsync(int id);
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }

    public interface IWatchlistRepository : IRepository<WatchlistItem>
    {
        Task<IEnumerable<WatchlistItem>> GetUserWatchlistAsync(int userId);
        Task<bool> IsInWatchlistAsync(int userId, int movieId);
    }

    public interface IPricingPlanRepository : IRepository<PricingPlan>
    {
    }
}
