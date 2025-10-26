using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class Actor : BaseAuditableEntity
    {

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

        // Many-to-many relationship with Movie through MovieActor join entity
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

        // Many-to-many relationship with TVSeries through TVSeriesActor join entity
        public ICollection<TVSeriesActor> TVSeriesActors { get; set; } = new List<TVSeriesActor>();

        [NotMapped]
        public int Age => DateTime.UtcNow.Year - BirthDate.Year;
    }
}
