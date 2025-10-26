using System;
using System.ComponentModel.DataAnnotations;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class Comment : BaseAuditableEntity
    {
        public int? MovieId { get; set; }

        public int? TVSeriesId { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;

        // Navigation properties
        public virtual Movie? Movie { get; set; }
        public virtual TVSeries? TVSeries { get; set; }
        public virtual User? User { get; set; }
    }
}
