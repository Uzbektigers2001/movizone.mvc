namespace MovizoneApp.Models
{
    public class TVSeries
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Seasons { get; set; }
        public int TotalEpisodes { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public string PosterImage { get; set; } = string.Empty; // Video player poster/thumbnail
        public string VideoUrl { get; set; } = string.Empty;
        public List<string> Actors { get; set; } = new List<string>();
        public List<Episode> Episodes { get; set; } = new List<Episode>();
        public bool IsFeatured { get; set; }
        public bool IsHidden { get; set; } // Hide from public listings
        public DateTime FirstAired { get; set; }
        public string Status { get; set; } = string.Empty; // "Ongoing" or "Completed"
    }
}
