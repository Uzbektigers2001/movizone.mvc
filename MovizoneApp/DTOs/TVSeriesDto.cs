using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading TVSeries data (GET operations)
    /// </summary>
    public class TVSeriesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public string PosterImage { get; set; } = string.Empty;
        public string BannerImage { get; set; } = string.Empty;
        public List<string> Actors { get; set; } = new List<string>();
        public int TotalSeasons { get; set; }
        public int Seasons { get; set; } // Alias for TotalSeasons
        public int TotalEpisodes { get; set; }
        public List<EpisodeDto> Episodes { get; set; } = new List<EpisodeDto>();
        public string Status { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public DateTime? FirstAired { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsHidden { get; set; }
        public bool ShowInBanner { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        // Soft delete fields (for admin visibility)
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }

    /// <summary>
    /// DTO for creating a new TVSeries (POST operations)
    /// </summary>
    public class CreateTVSeriesDto
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

        public List<string> Actors { get; set; } = new List<string>();

        [Range(1, 100)]
        public int TotalSeasons { get; set; }

        [Range(1, 100)]
        public int Seasons { get; set; } // Alias for TotalSeasons

        [Range(0, 1000)]
        public int TotalEpisodes { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Ongoing";

        [MaxLength(200)]
        public string Creator { get; set; } = string.Empty;

        public DateTime? FirstAired { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsHidden { get; set; }
        public bool ShowInBanner { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing TVSeries (PUT operations)
    /// </summary>
    public class UpdateTVSeriesDto
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

        public List<string> Actors { get; set; } = new List<string>();

        [Range(1, 100)]
        public int TotalSeasons { get; set; }

        [Range(1, 100)]
        public int Seasons { get; set; } // Alias for TotalSeasons

        [Range(0, 1000)]
        public int TotalEpisodes { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Ongoing";

        [MaxLength(200)]
        public string Creator { get; set; } = string.Empty;

        public DateTime? FirstAired { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsHidden { get; set; }
        public bool ShowInBanner { get; set; }
    }
}
