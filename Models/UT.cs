using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My_ticket.Models
{
    public class UT
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public int Quantity { get; set; }
        public bool IsPurchased { get; set; } = false;
        public bool IsRejected { get; set; }
        // ✅ العلاقة مع المستخدم
        [ForeignKey("UserId")]
        public User User { get; set; }

        // ✅ العلاقة مع التذكرة
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
        public int? Rating { get; set; } 
        public string? ReviewComment { get; set; } 

    }
}
