namespace BugTracker.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}