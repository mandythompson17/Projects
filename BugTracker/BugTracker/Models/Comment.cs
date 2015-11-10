using System.ComponentModel.DataAnnotations;
namespace BugTracker.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string TicketComment { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Attachment")]
        public string FileUrl { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}