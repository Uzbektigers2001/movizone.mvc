namespace MovizoneApp.Models
{
    public class PricingPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Period { get; set; } = string.Empty; // "month" or "year"
        public List<string> Features { get; set; } = new List<string>();
        public bool IsPopular { get; set; }
    }
}
