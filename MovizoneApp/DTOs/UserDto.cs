using System;
using System.ComponentModel.DataAnnotations;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for reading User data (GET operations)
    /// Excludes sensitive information like password
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string SubscriptionType { get; set; } = string.Empty;
        public DateTime? SubscriptionExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a new User (POST operations)
    /// </summary>
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        [MaxLength(50)]
        public string SubscriptionType { get; set; } = "Free";

        public DateTime? SubscriptionExpiry { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing User (PUT operations)
    /// </summary>
    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        [MaxLength(50)]
        public string SubscriptionType { get; set; } = "Free";

        public DateTime? SubscriptionExpiry { get; set; }
    }

    /// <summary>
    /// DTO for changing user password
    /// </summary>
    public class ChangePasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
