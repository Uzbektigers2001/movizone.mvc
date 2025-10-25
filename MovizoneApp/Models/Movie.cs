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
        public string VideoUrl { get; set; } = string.Empty;
        public List<string> Actors { get; set; } = new List<string>();
        public bool IsFeatured { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
