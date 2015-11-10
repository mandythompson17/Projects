using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public System.DateTimeOffset DateNotified { get; set; }
        public string CreatorUserId { get; set;}
        public string RecipientUserId { get; set; }
        public string Change { get; set; }
        public string Details { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
    }
}