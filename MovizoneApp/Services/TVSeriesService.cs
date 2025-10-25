using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class TVSeriesService : ITVSeriesService
    {
        private readonly List<TVSeries> _series;

        public TVSeriesService()
        {
            _series = new List<TVSeries>
            {
                new TVSeries
                {
                    Id = 1,
                    Title = "Breaking Bad",
                    Description = "A high school chemistry teacher turned methamphetamine producer partners with a former student.",
                    Year = 2008,
                    Rating = 9.5,
                    Genre = "Crime, Drama, Thriller",
                    Seasons = 5,
                    TotalEpisodes = 62,
                    Country = "USA",
                    Creator = "Vince Gilligan",
                    CoverImage = "/img/covers/cover10.jpg",
                    PosterImage = "/img/posters/poster10.jpg",
                    Actors = new List<string> { "Bryan Cranston", "Aaron Paul" },
                    Episodes = new List<Episode>
                    {
                        new Episode { Id = 1, TVSeriesId = 1, SeasonNumber = 1, EpisodeNumber = 1, Title = "Pilot", Description = "Diagnosed with terminal lung cancer, chemistry teacher Walter White teams up with former student Jesse Pinkman to cook and sell crystal meth.", Duration = 58, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", AirDate = new DateTime(2008, 1, 20) },
                        new Episode { Id = 2, TVSeriesId = 1, SeasonNumber = 1, EpisodeNumber = 2, Title = "Cat's in the Bag...", Description = "Walt and Jesse attempt to tie up loose ends. The desperate situation gets more complicated with the flip of a coin.", Duration = 48, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4", AirDate = new DateTime(2008, 1, 27) },
                        new Episode { Id = 3, TVSeriesId = 1, SeasonNumber = 1, EpisodeNumber = 3, Title = "...And the Bag's in the River", Description = "Walter fights with Jesse over his drug use, causing him to leave Walter alone to deal with their captive.", Duration = 48, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4", AirDate = new DateTime(2008, 2, 10) },
                        new Episode { Id = 4, TVSeriesId = 1, SeasonNumber = 1, EpisodeNumber = 4, Title = "Cancer Man", Description = "Walter finally tells his family about his cancer diagnosis. Jesse tries to make amends with his own parents.", Duration = 48, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", AirDate = new DateTime(2008, 2, 17) },
                        new Episode { Id = 5, TVSeriesId = 1, SeasonNumber = 1, EpisodeNumber = 5, Title = "Gray Matter", Description = "Walter and Skyler attend a former colleague's party. Jesse tries to free himself from drugs, while Skyler suspects Walter of hiding something.", Duration = 48, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4", AirDate = new DateTime(2008, 2, 24) },
                        new Episode { Id = 6, TVSeriesId = 1, SeasonNumber = 1, EpisodeNumber = 6, Title = "Crazy Handful of Nothin'", Description = "The side effects of chemo begin to plague Walt. Meanwhile, the DEA rounds up suspected dealers.", Duration = 48, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4", AirDate = new DateTime(2008, 3, 2) },
                        new Episode { Id = 7, TVSeriesId = 1, SeasonNumber = 1, EpisodeNumber = 7, Title = "A No-Rough-Stuff-Type Deal", Description = "Walter accepts his new identity as a drug dealer. Elsewhere, Jesse decides to put his aunt's house on the market.", Duration = 48, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", AirDate = new DateTime(2008, 3, 9) },
                        new Episode { Id = 8, TVSeriesId = 1, SeasonNumber = 2, EpisodeNumber = 1, Title = "Seven Thirty-Seven", Description = "Walt and Jesse realize how dire their situation is. They must come up with a plan to kill Tuco before he kills them first.", Duration = 47, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4", AirDate = new DateTime(2009, 3, 8) },
                        new Episode { Id = 9, TVSeriesId = 1, SeasonNumber = 2, EpisodeNumber = 2, Title = "Grilled", Description = "Walt and Jesse are held captive for Tuco. Marie and Hank comfort Skyler, who is distraught over Walt's disappearance.", Duration = 47, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4", AirDate = new DateTime(2009, 3, 15) },
                        new Episode { Id = 10, TVSeriesId = 1, SeasonNumber = 2, EpisodeNumber = 3, Title = "Bit by a Dead Bee", Description = "Walt and Jesse cover their tracks. Skyler becomes suspicious of Walt's actions.", Duration = 47, VideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", AirDate = new DateTime(2009, 3, 22) }
                    },
                    IsFeatured = true,
                    FirstAired = new DateTime(2008, 1, 20),
                    Status = "Completed"
                },
                new TVSeries
                {
                    Id = 2,
                    Title = "Game of Thrones",
                    Description = "Nine noble families fight for control over the lands of Westeros, while an ancient enemy returns after being dormant for millennia.",
                    Year = 2011,
                    Rating = 9.3,
                    Genre = "Action, Adventure, Drama",
                    Seasons = 8,
                    TotalEpisodes = 73,
                    Country = "USA",
                    Creator = "David Benioff, D.B. Weiss",
                    CoverImage = "/img/covers/cover11.jpg",
                    PosterImage = "/img/posters/poster11.jpg",
                    Actors = new List<string> { "Emilia Clarke", "Kit Harington" },
                    IsFeatured = true,
                    FirstAired = new DateTime(2011, 4, 17),
                    Status = "Completed"
                },
                new TVSeries
                {
                    Id = 3,
                    Title = "Stranger Things",
                    Description = "When a young boy disappears, his mother, a police chief and his friends must confront terrifying supernatural forces.",
                    Year = 2016,
                    Rating = 8.7,
                    Genre = "Drama, Fantasy, Horror",
                    Seasons = 4,
                    TotalEpisodes = 34,
                    Country = "USA",
                    Creator = "Matt Duffer, Ross Duffer",
                    CoverImage = "/img/covers/cover13.jpg",
                    Actors = new List<string> { "Millie Bobby Brown", "Finn Wolfhard" },
                    IsFeatured = true,
                    FirstAired = new DateTime(2016, 7, 15),
                    Status = "Ongoing"
                },
                new TVSeries
                {
                    Id = 4,
                    Title = "The Crown",
                    Description = "Follows the political rivalries and romance of Queen Elizabeth II's reign and the events that shaped the second half of the 20th century.",
                    Year = 2016,
                    Rating = 8.6,
                    Genre = "Biography, Drama, History",
                    Seasons = 6,
                    TotalEpisodes = 60,
                    Country = "UK",
                    Creator = "Peter Morgan",
                    CoverImage = "/img/covers/cover14.jpg",
                    Actors = new List<string> { "Claire Foy", "Olivia Colman" },
                    IsFeatured = false,
                    FirstAired = new DateTime(2016, 11, 4),
                    Status = "Completed"
                }
            };
        }

        public List<TVSeries> GetAllSeries() => _series;

        public TVSeries? GetSeriesById(int id) => _series.FirstOrDefault(s => s.Id == id);

        public List<TVSeries> GetFeaturedSeries() => _series.Where(s => s.IsFeatured).ToList();

        public void AddSeries(TVSeries series)
        {
            series.Id = _series.Any() ? _series.Max(s => s.Id) + 1 : 1;
            _series.Add(series);
        }

        public void UpdateSeries(TVSeries series)
        {
            var existingSeries = GetSeriesById(series.Id);
            if (existingSeries != null)
            {
                existingSeries.Title = series.Title;
                existingSeries.Description = series.Description;
                existingSeries.Year = series.Year;
                existingSeries.Rating = series.Rating;
                existingSeries.Genre = series.Genre;
                existingSeries.Seasons = series.Seasons;
                existingSeries.TotalEpisodes = series.TotalEpisodes;
                existingSeries.Country = series.Country;
                existingSeries.Creator = series.Creator;
                existingSeries.CoverImage = series.CoverImage;
                existingSeries.Actors = series.Actors;
                existingSeries.IsFeatured = series.IsFeatured;
                existingSeries.FirstAired = series.FirstAired;
                existingSeries.Status = series.Status;
            }
        }

        public void DeleteSeries(int id)
        {
            var series = GetSeriesById(id);
            if (series != null)
            {
                _series.Remove(series);
            }
        }
    }
}
