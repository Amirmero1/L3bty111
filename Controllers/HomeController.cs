using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My_ticket.Data;
using My_ticket.Models;
using System.Diagnostics;

namespace My_ticket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }


        public IActionResult Index()
        {
            var tickets = _db.Tickets.ToList(); // جلب كل التذاكر من قاعدة البيانات
            return View(tickets); // تمريرها للـ View
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult ReturnPolicy()
        {
            return View();
        }

    }
}
