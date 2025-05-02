using Microsoft.AspNetCore.Identity;

namespace SmartCitizen.Models
{
    public class Comment
    {
        public int Id { get; set; }  // Unique identifier for each comment
        public string UserId { get; set; }  // Foreign key for the user who made the comment
        public IdentityUser ?User { get; set; } // Navigation property to access user details
        public int ComplaintId { get; set; }  // Foreign key for the complaint
        public Complaint? Complaint { get; set; } // Navigation property to link complaint
        public DateTime CreatedAt { get; set; }  // Timestamp for the comment
        public string Content { get; set; }  // The actual text content of the comment
    }

}
