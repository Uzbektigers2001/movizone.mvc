using System;

namespace MovizoneApp.Core.Models
{
    /// <summary>
    /// Base class for all entities with auditing and soft delete support
    /// </summary>
    public abstract class BaseAuditableEntity
    {
        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        public int Id { get; set; }

        // Audit Properties

        /// <summary>
        /// Date and time when the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID of the user who created this entity
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ID of the user who last updated this entity
        /// </summary>
        public int? UpdatedBy { get; set; }

        // Soft Delete Properties

        /// <summary>
        /// Indicates whether this entity has been soft deleted
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Date and time when the entity was soft deleted
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// ID of the user who deleted this entity
        /// </summary>
        public int? DeletedBy { get; set; }
    }
}
