using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BugTracker.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.Comments = new HashSet<Comment>();
            this.Histories = new HashSet<TicketHistory>();
        }

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public Nullable<System.DateTimeOffset> Updated {get; set;}
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Type")]
        public int TicketTypeId { get; set; }
        [Display(Name = "Priority")]
        public int TicketPriorityId { get; set; }
        [Display(Name = "Status")]
        public int TicketStatusId { get; set; }
        [Display(Name = "Owner")]
        public string OwnerUserId { get; set; }
        [Display(Name = "Assigned To")]
        public string AssignedToUserId { get; set; }
        [Display(Name = "Attachment")]
        public string FileUrl { get; set; }

        public virtual Project Project { get; set; }
        public virtual TicketType Type { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<TicketHistory> Histories { get; set; }
    }
}