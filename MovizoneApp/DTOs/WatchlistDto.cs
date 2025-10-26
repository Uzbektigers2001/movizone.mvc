using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading Watchlist data (GET operations)
    /// </summary>
    public class WatchlistDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string MovieCoverImage { get; set; } = string.Empty;
        public string MovieGenre { get; set; } = string.Empty;
        public double MovieRating { get; set; }
        public DateTime AddedAt { get; set; }

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
    /// DTO for adding item to watchlist (POST operations)
    /// </summary>
    public class CreateWatchlistItemDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int MovieId { get; set; }
    }
}
