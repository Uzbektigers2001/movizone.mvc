using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovizoneApp.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Bio { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Photo { get; set; } = string.Empty;

        // Navigation properties for many-to-many relationships
        public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
        public ICollection<ActorTVSeries> ActorTVSeries { get; set; } = new List<ActorTVSeries>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public int Age => DateTime.UtcNow.Year - BirthDate.Year;
    }
}
