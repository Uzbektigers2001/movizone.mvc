using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.Models
{
    public class TVSeries
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Range(0, 10)]
        public double Rating { get; set; }

        [Required]
        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Range(1, 100)]
        public int Seasons { get; set; }

        [Range(1, 10000)]
        public int TotalEpisodes { get; set; }

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Creator { get; set; } = string.Empty;

        [MaxLength(500)]
        public string CoverImage { get; set; } = string.Empty;

        public List<string> Actors { get; set; } = new List<string>();
        public bool IsFeatured { get; set; }
        public DateTime FirstAired { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // "Ongoing" or "Completed"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
