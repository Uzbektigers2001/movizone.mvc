using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

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
    }
}
