using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS_Web.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Role: "Lecturer", "Coordinator", "Admin" etc.
        public string Role { get; set; } = "Lecturer";

        // Friendly name
        public string? FullName { get; set; }
    }
}
