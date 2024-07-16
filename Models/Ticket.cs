using System;

namespace MESDashboard.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Priority { get; set; }
        public string Issue { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
