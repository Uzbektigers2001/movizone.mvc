using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.Models
{
    /// <summary>
    /// Join entity for TVSeries-Actor many-to-many relationship
    /// </summary>
    public class TVSeriesActor
    {
        public int TVSeriesId { get; set; }
        public TVSeries TVSeries { get; set; } = null!;

        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;

        [MaxLength(100)]
        public string Role { get; set; } = string.Empty; // Character name/role in the series

        public int DisplayOrder { get; set; } // Order in which actors are displayed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
