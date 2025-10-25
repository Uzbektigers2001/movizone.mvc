using System;
using System.Collections.Generic;

namespace MovizoneApp.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Duration { get; set; } // in minutes
        public string Country { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public string PosterImage { get; set; } = string.Empty; // Video player poster/thumbnail
        public string BannerImage { get; set; } = string.Empty; // Banner/hero image for home page
        public string VideoUrl { get; set; } = string.Empty;
        public List<string> Actors { get; set; } = new List<string>();
        public bool IsFeatured { get; set; }
        public bool IsHidden { get; set; } // Hide from public listings
        public bool ShowInBanner { get; set; } // Show in home page banner carousel
        public DateTime ReleaseDate { get; set; }
    }
}
