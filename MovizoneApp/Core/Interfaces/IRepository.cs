using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

    public interface IMovieRepository : IRepository<Models.Movie>
    {
        Task<IEnumerable<Models.Movie>> GetFeaturedMoviesAsync();
        Task<IEnumerable<Models.Movie>> SearchMoviesAsync(string? searchTerm, string? genre);
    }

    public interface ITVSeriesRepository : IRepository<Models.TVSeries>
    {
        Task<IEnumerable<Models.TVSeries>> GetFeaturedSeriesAsync();
        Task<IEnumerable<Models.TVSeries>> SearchSeriesAsync(string? searchTerm, string? genre);
    }

    public interface IActorRepository : IRepository<Models.Actor>
    {
        Task<Models.Actor?> GetActorWithDetailsAsync(int id);
    }

    public interface IUserRepository : IRepository<Models.User>
    {
        Task<Models.User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }

    public interface IReviewRepository : IRepository<Models.Review>
    {
        Task<IEnumerable<Models.Review>> GetReviewsByMovieIdAsync(int movieId);
        Task<double> GetAverageRatingAsync(int movieId);
    }

    public interface IWatchlistRepository : IRepository<Models.WatchlistItem>
    {
        Task<IEnumerable<Models.WatchlistItem>> GetUserWatchlistAsync(int userId);
        Task<bool> IsInWatchlistAsync(int userId, int movieId);
    }

    public interface IPricingPlanRepository : IRepository<Models.PricingPlan>
    {
    }
}
