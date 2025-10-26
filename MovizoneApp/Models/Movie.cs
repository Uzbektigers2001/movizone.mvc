using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.Models
{
    public class Movie
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

        [Range(1, 500)]
        public int Duration { get; set; } // in minutes

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Director { get; set; } = string.Empty;

        [MaxLength(500)]
        public string CoverImage { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PosterImage { get; set; } = string.Empty;

        [MaxLength(500)]
        public string BannerImage { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string VideoUrl { get; set; } = string.Empty;

        public List<string> Actors { get; set; } = new List<string>();
        public bool IsFeatured { get; set; }
        public bool IsHidden { get; set; } // Hide from public listings
        public bool ShowInBanner { get; set; } // Show in home page banner carousel
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
