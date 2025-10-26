using System;
using System.ComponentModel.DataAnnotations;
using MovizoneApp.Core.Models;

namespace MovizoneApp.Models
{
    public class User : BaseAuditableEntity
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // Hashed password

        [MaxLength(50)]
        public string Role { get; set; } = "User"; // User, Admin

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string Avatar { get; set; } = "/img/user.svg";

        /// <summary>
        /// Sets the password with BCrypt hashing
        /// </summary>
        public void SetPassword(string plainPassword)
        {
            Password = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        /// <summary>
        /// Verifies if the provided password matches the hashed password
        /// </summary>
        public bool VerifyPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, Password);
        }
    }
}
