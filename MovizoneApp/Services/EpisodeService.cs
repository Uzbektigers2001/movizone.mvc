using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class EpisodeService : IEpisodeService
    {
        private readonly List<Episode> _episodes;
        private readonly ITVSeriesService _seriesService;

        public EpisodeService(ITVSeriesService seriesService)
        {
            _seriesService = seriesService;
            _episodes = new List<Episode>();

            // Initialize with episodes from all series
            foreach (var series in _seriesService.GetAllSeries())
            {
                _episodes.AddRange(series.Episodes);
            }
        }

        public List<Episode> GetAllEpisodes() => _episodes;

        public Episode? GetEpisodeById(int id) => _episodes.FirstOrDefault(e => e.Id == id);

        public List<Episode> GetEpisodesBySeriesId(int seriesId) =>
            _episodes.Where(e => e.TVSeriesId == seriesId).OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber).ToList();

        public List<Episode> GetEpisodesBySeasonAndSeries(int seriesId, int seasonNumber) =>
            _episodes.Where(e => e.TVSeriesId == seriesId && e.SeasonNumber == seasonNumber).OrderBy(e => e.EpisodeNumber).ToList();

        public void AddEpisode(Episode episode)
        {
            episode.Id = _episodes.Any() ? _episodes.Max(e => e.Id) + 1 : 1;
            _episodes.Add(episode);

            // Also add to the series
            var series = _seriesService.GetSeriesById(episode.TVSeriesId);
            if (series != null)
            {
                series.Episodes.Add(episode);
            }
        }

        public void UpdateEpisode(Episode episode)
        {
            var existingEpisode = GetEpisodeById(episode.Id);
            if (existingEpisode != null)
            {
                existingEpisode.TVSeriesId = episode.TVSeriesId;
                existingEpisode.SeasonNumber = episode.SeasonNumber;
                existingEpisode.EpisodeNumber = episode.EpisodeNumber;
                existingEpisode.Title = episode.Title;
                existingEpisode.Description = episode.Description;
                existingEpisode.Duration = episode.Duration;
                existingEpisode.VideoUrl = episode.VideoUrl;
                existingEpisode.ThumbnailImage = episode.ThumbnailImage;
                existingEpisode.AirDate = episode.AirDate;

                // Update in series
                var series = _seriesService.GetSeriesById(episode.TVSeriesId);
                if (series != null)
                {
                    var seriesEpisode = series.Episodes.FirstOrDefault(e => e.Id == episode.Id);
                    if (seriesEpisode != null)
                    {
                        seriesEpisode.TVSeriesId = episode.TVSeriesId;
                        seriesEpisode.SeasonNumber = episode.SeasonNumber;
                        seriesEpisode.EpisodeNumber = episode.EpisodeNumber;
                        seriesEpisode.Title = episode.Title;
                        seriesEpisode.Description = episode.Description;
                        seriesEpisode.Duration = episode.Duration;
                        seriesEpisode.VideoUrl = episode.VideoUrl;
                        seriesEpisode.ThumbnailImage = episode.ThumbnailImage;
                        seriesEpisode.AirDate = episode.AirDate;
                    }
                }
            }
        }

        public void DeleteEpisode(int id)
        {
            var episode = GetEpisodeById(id);
            if (episode != null)
            {
                _episodes.Remove(episode);

                // Remove from series
                var series = _seriesService.GetSeriesById(episode.TVSeriesId);
                if (series != null)
                {
                    var seriesEpisode = series.Episodes.FirstOrDefault(e => e.Id == id);
                    if (seriesEpisode != null)
                    {
                        series.Episodes.Remove(seriesEpisode);
                    }
                }
            }
        }
    }
}
