using Microsoft.EntityFrameworkCore;
using My_ticket.Models;
namespace My_ticket.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> user1 { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UT> UTS { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=Login;Integrated Security=true;Encrypt=false");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // علاقة المستخدم بالتذاكر عبر UT
            modelBuilder.Entity<UT>()
                .HasKey(ut => ut.Id);

            modelBuilder.Entity<UT>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTickets)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UT>()
                .HasOne(ut => ut.Ticket)
                .WithMany()
                .HasForeignKey(ut => ut.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // تحديد القيم الافتراضية
            modelBuilder.Entity<Ticket>()
                .Property(t => t.CreateTime)
                .HasDefaultValueSql("GETDATE()"); // يتم ضبط تاريخ الإنشاء تلقائيًا

            modelBuilder.Entity<UT>()
                .Property(ut => ut.IsPurchased)
                .HasDefaultValue(false);
        }
    }
}
