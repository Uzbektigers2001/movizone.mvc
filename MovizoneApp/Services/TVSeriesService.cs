using System;
using System.Collections.Generic;
using System.Linq;
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
