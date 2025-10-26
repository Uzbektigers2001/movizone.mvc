using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.Models
{
    /// <summary>
    /// Junction table for many-to-many relationship between Actors and TV Series
    /// </summary>
    public class ActorTVSeries
    {
        public int Id { get; set; }

        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;

        public int TVSeriesId { get; set; }
        public TVSeries TVSeries { get; set; } = null!;

        [MaxLength(200)]
        public string Role { get; set; } = string.Empty; // Character name

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
