using System;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class WatchlistItem : BaseAuditableEntity
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
