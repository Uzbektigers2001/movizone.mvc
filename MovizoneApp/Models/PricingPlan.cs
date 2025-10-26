using System.Collections.Generic;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class PricingPlan : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Period { get; set; } = string.Empty; // "month" or "year"
        public List<string> Features { get; set; } = new List<string>();
        public bool IsPopular { get; set; }
    }
}
