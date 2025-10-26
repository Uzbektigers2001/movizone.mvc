namespace MovizoneApp.Models
{
    public class ActorMovie
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public int MovieId { get; set; }
        public string Role { get; set; } = string.Empty; // Character name
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
