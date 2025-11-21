using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_Web.Models
{
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Month { get; set; } = string.Empty;

        [Required]
        public int HoursWorked { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        public string? Description { get; set; }

        public string? UploadedFileName { get; set; }

        public DateTime DateSubmitted { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        // Not mapped to DB (computed)
        [NotMapped]
        public decimal Payment => HoursWorked * HourlyRate;
    }
}
