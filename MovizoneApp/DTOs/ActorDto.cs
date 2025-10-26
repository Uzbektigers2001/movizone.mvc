using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading Actor data (GET operations)
    /// </summary>
    public class ActorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public List<string> Movies { get; set; } = new List<string>();
        public List<string> TVSeries { get; set; } = new List<string>();

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        // Soft delete fields (for admin visibility)
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Actor (POST operations)
    /// </summary>
    public class CreateActorDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Biography { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        [MaxLength(100)]
        public string Nationality { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Photo { get; set; } = string.Empty;

        public List<string> Movies { get; set; } = new List<string>();
        public List<string> TVSeries { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO for updating an existing Actor (PUT operations)
    /// </summary>
    public class UpdateActorDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Biography { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        [MaxLength(100)]
        public string Nationality { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Photo { get; set; } = string.Empty;

        public List<string> Movies { get; set; } = new List<string>();
        public List<string> TVSeries { get; set; } = new List<string>();
    }
}
