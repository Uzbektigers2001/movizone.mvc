using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface ITVSeriesService
    {
        List<TVSeries> GetAllSeries();
        TVSeries? GetSeriesById(int id);
        List<TVSeries> GetFeaturedSeries();
        void AddSeries(TVSeries series);
        void UpdateSeries(TVSeries series);
        void DeleteSeries(int id);
    }
}
