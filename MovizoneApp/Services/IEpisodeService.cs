using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IEpisodeService
    {
        List<Episode> GetAllEpisodes();
        Episode? GetEpisodeById(int id);
        List<Episode> GetEpisodesBySeriesId(int seriesId);
        List<Episode> GetEpisodesBySeasonAndSeries(int seriesId, int seasonNumber);
        void AddEpisode(Episode episode);
        void UpdateEpisode(Episode episode);
        void DeleteEpisode(int id);
    }
}
