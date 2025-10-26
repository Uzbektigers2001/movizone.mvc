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
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Review (POST operations)
    /// </summary>
    public class CreateReviewDto
    {
        [Required]
        public int MovieId { get; set; }

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
