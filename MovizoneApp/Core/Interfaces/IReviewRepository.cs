using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Core.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        // Movie reviews
        Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(int movieId);
        Task<double> GetAverageRatingAsync(int movieId);

        // TVSeries reviews
        Task<IEnumerable<Review>> GetReviewsByTVSeriesIdAsync(int tvSeriesId);
        Task<double> GetAverageRatingByTVSeriesIdAsync(int tvSeriesId);
    }
}
