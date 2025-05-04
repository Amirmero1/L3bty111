namespace My_ticket.Request
{
    public class CreateTicketRequest
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Age { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public int personId { get; set; } = 1;
        public int Rating { get; set; } = 5;
        public string ReviewComment { get; set; } = "لا يوجد";

    }
}
