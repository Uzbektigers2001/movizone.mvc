using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading PricingPlan data (GET operations)
    /// </summary>
    public class PricingPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Duration { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new List<string>();
        public bool IsPopular { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a new PricingPlan (POST operations)
    /// </summary>
    public class CreatePricingPlanDto
    {
        [Required(ErrorMessage = "Plan name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 99999.99)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string Duration { get; set; } = string.Empty;

        public List<string> Features { get; set; } = new List<string>();

        public bool IsPopular { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing PricingPlan (PUT operations)
    /// </summary>
    public class UpdatePricingPlanDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Plan name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 99999.99)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string Duration { get; set; } = string.Empty;

        public List<string> Features { get; set; } = new List<string>();

        public bool IsPopular { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
