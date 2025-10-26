using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading Movie data (GET operations)
    /// </summary>
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public string PosterImage { get; set; } = string.Empty;
        public string BannerImage { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public List<string> Actors { get; set; } = new List<string>();
        public bool IsFeatured { get; set; }
        public bool IsHidden { get; set; }
        public bool ShowInBanner { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Movie (POST operations)
    /// </summary>
    public class CreateMovieDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Required]
        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int Duration { get; set; }

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
        public bool IsHidden { get; set; }
        public bool ShowInBanner { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing Movie (PUT operations)
    /// </summary>
    public class UpdateMovieDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Required]
        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int Duration { get; set; }

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
        public bool IsHidden { get; set; }
        public bool ShowInBanner { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
