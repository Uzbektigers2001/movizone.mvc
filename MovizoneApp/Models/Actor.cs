namespace MovizoneApp.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public List<string> Movies { get; set; } = new List<string>();
        public List<string> TVSeries { get; set; } = new List<string>();
        public int? Age => BirthDate.HasValue ? DateTime.Now.Year - BirthDate.Value.Year : null;
    }
}
