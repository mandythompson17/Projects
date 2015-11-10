using System;
namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string OldDisplayValue { get; set; }
        public string NewDisplayValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTimeOffset DateChanged { get; set; }
        public string UserId { get; set; }
        public string EditId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}