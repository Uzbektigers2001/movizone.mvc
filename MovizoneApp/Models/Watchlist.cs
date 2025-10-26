using System;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class WatchlistItem : BaseAuditableEntity
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
    }
}
