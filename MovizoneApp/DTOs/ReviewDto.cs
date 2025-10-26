using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading Review data (GET operations)
    /// </summary>
    public class ReviewDto
    {
        public int Id { get; set; }

        // Either MovieId OR TVSeriesId will be set
        public int? MovieId { get; set; }
        public int? TVSeriesId { get; set; }

        public string ContentTitle { get; set; } = string.Empty; // Movie or Series title
        public string ContentType { get; set; } = string.Empty; // "Movie" or "TVSeries"

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

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
    /// DTO for creating a new Review (POST operations)
    /// Either MovieId OR TVSeriesId must be set, not both
    /// </summary>
    public class CreateReviewDto
    {
        public int? MovieId { get; set; }
        public int? TVSeriesId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing Review (PUT operations)
    /// </summary>
    public class UpdateReviewDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}
