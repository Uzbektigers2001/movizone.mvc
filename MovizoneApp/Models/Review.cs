namespace MovizoneApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int? MovieId { get; set; } // Nullable - for movies
        public int? TVSeriesId { get; set; } // Nullable - for TV series
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-10
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Helper properties
        public string ContentType => MovieId.HasValue ? "Movie" : "TVSeries";
        public int ContentId => MovieId ?? TVSeriesId ?? 0;
    }
}
