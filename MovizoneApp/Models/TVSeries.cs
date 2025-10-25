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
        public string VideoUrl { get; set; } = string.Empty;
        public List<string> Actors { get; set; } = new List<string>();
        public bool IsFeatured { get; set; }
        public DateTime FirstAired { get; set; }
        public string Status { get; set; } = string.Empty; // "Ongoing" or "Completed"
    }
}
