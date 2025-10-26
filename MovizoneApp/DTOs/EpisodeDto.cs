using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading Episode data (GET operations)
    /// </summary>
    public class EpisodeDto
    {
        public int Id { get; set; }
        public int TVSeriesId { get; set; }
        public string SeriesTitle { get; set; } = string.Empty;
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string ThumbnailImage { get; set; } = string.Empty;
        public DateTime AirDate { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Episode (POST operations)
    /// </summary>
    public class CreateEpisodeDto
    {
        [Required(ErrorMessage = "TV Series ID is required")]
        public int TVSeriesId { get; set; }

        [Required(ErrorMessage = "Season number is required")]
        [Range(1, 100, ErrorMessage = "Season number must be between 1 and 100")]
        public int SeasonNumber { get; set; }

        [Required(ErrorMessage = "Episode number is required")]
        [Range(1, 1000, ErrorMessage = "Episode number must be between 1 and 1000")]
        public int EpisodeNumber { get; set; }

        [Required(ErrorMessage = "Episode title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int Duration { get; set; }

        [MaxLength(1000)]
        public string VideoUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ThumbnailImage { get; set; } = string.Empty;

        public DateTime? AirDate { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing Episode (PUT operations)
    /// </summary>
    public class UpdateEpisodeDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "TV Series ID is required")]
        public int TVSeriesId { get; set; }

        [Required(ErrorMessage = "Season number is required")]
        [Range(1, 100, ErrorMessage = "Season number must be between 1 and 100")]
        public int SeasonNumber { get; set; }

        [Required(ErrorMessage = "Episode number is required")]
        [Range(1, 1000, ErrorMessage = "Episode number must be between 1 and 1000")]
        public int EpisodeNumber { get; set; }

        [Required(ErrorMessage = "Episode title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int Duration { get; set; }

        [MaxLength(1000)]
        public string VideoUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ThumbnailImage { get; set; } = string.Empty;

        public DateTime? AirDate { get; set; }
    }
}
