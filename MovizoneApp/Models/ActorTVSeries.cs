namespace MovizoneApp.Models
{
    public class ActorTVSeries
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public int TVSeriesId { get; set; }
        public string Role { get; set; } = string.Empty; // Character name
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
