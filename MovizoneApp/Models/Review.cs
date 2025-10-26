using System;
using System.ComponentModel.DataAnnotations;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class Review : BaseAuditableEntity
    {
        // Either MovieId OR TVSeriesId should be set, not both
        public int? MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int? TVSeriesId { get; set; }
        public TVSeries? TVSeries { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Rating { get; set; } // 1-10

        /// <summary>
        /// Helper property to get content type
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string ContentType => MovieId.HasValue ? "Movie" : "TVSeries";

        /// <summary>
        /// Helper property to get content ID
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int ContentId => MovieId ?? TVSeriesId ?? 0;
    }
}
