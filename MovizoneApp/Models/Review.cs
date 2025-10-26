using System;
using System.ComponentModel.DataAnnotations;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class Review : BaseAuditableEntity
    {

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
    }
}
