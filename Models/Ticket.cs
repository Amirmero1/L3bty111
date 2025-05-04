namespace My_ticket.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public double Price { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public State State { get; set; }
        public int? personId { get; set; }
        public User person { get; set; }
        public string ImageUrl { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Age { get; set; }
        public string Type { get; set; }
        public bool IsInCart { get; set; } = false;
        public int Quantity { get; set; } = 1;
        public int? Rating { get; set; } 
        public string ReviewComment { get; set; }
    }

    public enum State
    {
        Pending,
        Completed
    }
}

