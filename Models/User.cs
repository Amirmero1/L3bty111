namespace My_ticket.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public string Address { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<UT> UserTickets { get; set; } = new List<UT>();
        public int? Rating { get; set; }
        public string ReviewComment { get; set; }
    }

    public enum Role
    {
        personer,
        person
    }
}
