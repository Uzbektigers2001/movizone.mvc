namespace MovizoneApp.Models
{
    public class WatchlistItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
