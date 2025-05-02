using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCitizen.Models
{
    public class Complaint
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        public string? ImagePath { get; set; } // Stores image file path

        public string Location { get; set; } // Stores the location of the complaint
    }
}
