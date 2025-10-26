using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading Comment data (GET operations)
    /// </summary>
    public class CommentDto
    {
        public int Id { get; set; }
        public int? MovieId { get; set; }
        public int? TVSeriesId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string ContentType { get; set; } = string.Empty; // "Movie" or "TV Series"
        public string ContentTitle { get; set; } = string.Empty;

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Comment (POST operations)
    /// </summary>
    public class CreateCommentDto
    {
        public int? MovieId { get; set; }
        public int? TVSeriesId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Comment text is required")]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;
    }

    /// <summary>
    /// DTO for updating an existing Comment (PUT operations)
    /// </summary>
    public class UpdateCommentDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment text is required")]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        public bool IsApproved { get; set; }
    }
}
