using System;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class Episode : BaseAuditableEntity
    {
        public int TVSeriesId { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; } // in minutes
        public string VideoUrl { get; set; } = string.Empty;
        public string ThumbnailImage { get; set; } = string.Empty;
        public DateTime? AirDate { get; set; }
    }
}
