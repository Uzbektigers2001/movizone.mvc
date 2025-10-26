using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int? MovieId { get; set; } // Nullable - for movies
        public int? TVSeriesId { get; set; } // Nullable - for TV series

        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Rating { get; set; } // 1-10

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Helper properties
        public string ContentType => MovieId.HasValue ? "Movie" : "TVSeries";
        public int ContentId => MovieId ?? TVSeriesId ?? 0;
    }
}
