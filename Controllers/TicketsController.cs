using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using My_ticket.Data;
using My_ticket.Models;
using My_ticket.Request;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace My_ticket.Controllers
{
    public class TicketsController : Controller
    {
        private readonly AppDbContext _db;

        public TicketsController(AppDbContext db)
        {
            _db = db;
        }

        private int? userId
        {
            get
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : (int?)null;
            }
        }

        public IActionResult Index()
        {
            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = _db.Tickets.ToList();
            var uts = _db.UTS.ToList();

            ViewBag.UTS = uts;

            return View(tickets);
        }


        [Authorize(Roles = $"{nameof(Role.personer)}")]
        [HttpGet]
        public IActionResult Create()
        {
            var persons = _db.user1.Where(x => x.Role == Models.Role.person).ToList();
            ViewBag.persons = persons;
            return View();
        }

        [Authorize(Roles = $"{nameof(Role.personer)}")]
        [HttpPost]
        public IActionResult Create(CreateTicketRequest request)
        {
            var ticket = new Ticket()
            {
                Name = request.Name,
                personId = 1,
                Details = request.Details,
                Price = request.Price,
                CreateTime = DateTime.Now,
                State = State.Pending,
                ImageUrl = request.ImageUrl,
                Age = request.Age,
                IsInCart = false,
                Quantity = request.Quantity,
                Type = request.Type,
                Image1 = request.Image1,
                Image2 = request.Image2,
                Rating = 5,
                ReviewComment = "لا يوجد",
            };

            _db.Tickets.Add(ticket);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = $"{nameof(Role.person)}")]
        public IActionResult CompleteTicket(int id)
        {
            var ticket = _db.Tickets.FirstOrDefault(x => x.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.State = State.Completed;
            ticket.CompletionTime = DateTime.Now;
            _db.Tickets.Update(ticket);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult My_Tickets()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                ViewBag.Message = "يجب تسجيل الدخول لعرض التذاكر!";
                return View(new List<Ticket>());
            }

            var user = _db.user1.FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                ViewBag.Message = "حدث خطأ أثناء جلب بيانات المستخدم!";
                return View(new List<Ticket>());
            }

            // ✅ جلب التذاكر الغير مرفوضة فقط من UTS
            var cartItems = _db.UTS
                .Where(ut => ut.UserId == user.Id && !ut.IsPurchased && !ut.IsRejected) // 🔹 استبعاد التذاكر المرفوضة
                .Select(ut => ut.Ticket)
                .ToList();

            if (!cartItems.Any())
            {
                ViewBag.Message = "السلة فارغة، أضف بعض التذاكر!";
            }

            return View(cartItems);
        }




        public IActionResult Details(int id)
        {
            var ticket = _db.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public IActionResult Cart()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                ViewBag.Message = "يجب تسجيل الدخول لعرض السلة!";
                return View(new List<Ticket>());
            }

            var user = _db.user1.FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                ViewBag.Message = "حدث خطأ أثناء جلب بيانات المستخدم!";
                return View(new List<Ticket>());
            }

            // ✅ جلب التذاكر الغير مرفوضة فقط
            var ticketsInCart = _db.UTS
                .Where(ut => ut.UserId == user.Id && !ut.IsPurchased && !ut.IsRejected) // 🔹 استبعاد التذاكر المرفوضة
                .Include(ut => ut.Ticket)
                .Select(ut => ut.Ticket)
                .ToList();

            Console.WriteLine($"📌 عدد التذاكر في سلة المستخدم {user.FullName}: {ticketsInCart.Count}");

            if (!ticketsInCart.Any())
            {
                ViewBag.Message = "السلة فارغة!";
            }

            return View(ticketsInCart);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Json(new { count = 0 });
            }

            var user = _db.user1.FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                return Json(new { count = 0 });
            }

            int cartCount = _db.UTS.Count(ut => ut.UserId == user.Id && !ut.IsPurchased && !ut.IsRejected);
            return Json(new { count = cartCount });
        }



        [Authorize]
        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var user = _db.user1.SingleOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                return Json(new { success = false, message = "يجب تسجيل الدخول أولاً" });
            }

            var ticket = _db.Tickets.Find(id);
            if (ticket == null)
            {
                return Json(new { success = false, message = "لم يتم العثور على التذكرة" });
            }

            var cartItem = _db.UTS.FirstOrDefault(ut => ut.UserId == user.Id && ut.TicketId == ticket.Id);
            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                _db.UTS.Add(new UT
                {
                    UserId = user.Id,
                    TicketId = ticket.Id,
                    Quantity = 1,
                    IsPurchased = false
                });
            }

            _db.SaveChanges();
            return Json(new { success = true, message = "تمت إضافة التذكرة بنجاح إلى السلة" });
        }



        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Json(new { success = false, message = "يجب تسجيل الدخول أولاً" });
            }

            var user = _db.user1.SingleOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                return Json(new { success = false, message = "حدث خطأ، لم يتم العثور على المستخدم" });
            }

            var cartItem = _db.UTS.FirstOrDefault(ut => ut.UserId == user.Id && ut.TicketId == id);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity; // ✅ تحديث الكمية الجديدة
                _db.Entry(cartItem).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true, message = "تم تحديث الكمية بنجاح" });
            }

            return Json(new { success = false, message = "لم يتم العثور على التذكرة في السلة" });
        }


        public IActionResult RemoveFromCart(int id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Cart");
            }

            var user = _db.user1.SingleOrDefault(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Cart");
            }

            try
            {
                var cartItem = _db.UTS.FirstOrDefault(ut => ut.UserId == user.Id && ut.TicketId == id);

                if (cartItem != null)
                {
                    _db.UTS.Remove(cartItem);
                    int result = _db.SaveChanges();

                    if (result > 0)
                    {
                        Console.WriteLine($"🛠 تم حذف التذكرة {id} من سلة المستخدم {user.FullName}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ لم يتم حذف التذكرة {id} بسبب مشكلة في الحفظ");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ لم يتم العثور على التذكرة {id} في سلة المستخدم {user.FullName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء حذف التذكرة: {ex.Message}");
            }

            return RedirectToAction("Cart");
        }
        public async Task<IActionResult> Track()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Home"); // الرجوع للصفحة الرئيسية إذا لم يكن المستخدم مسجّل الدخول
            }

            var user = await _db.user1.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home"); // الرجوع إذا لم يتم العثور على المستخدم
            }

            // ✅ جلب البيانات من `UTS` وليس `Tickets`
            var userOrders = await _db.UTS
                .Where(ut => ut.UserId == user.Id)
                .Include(ut => ut.Ticket) // تحميل بيانات التذكرة لكل طلب
                .ToListAsync();

            return View(userOrders); // ✅ الآن يتم إرسال `List<UT>` بدلاً من `List<Ticket>`
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderCount()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Json(new { count = 0 }); // لو المستخدم مش مسجل دخول، رجّع صفر
            }

            var user = await _db.user1.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                return Json(new { count = 0 });
            }

            int orderCount = await _db.UTS
                .Where(ut => ut.UserId == user.Id && ut.IsPurchased) // ✅ فقط التذاكر المشتراة
                .CountAsync();

            return Json(new { count = orderCount });
        }
        [HttpGet]
        [Authorize(Roles = "personer")] // ✅ السماح فقط للمسؤول
        public JsonResult GetPendingOrdersCount()
        {
            var pendingOrdersCount = _db.UTS
                .Where(o => !o.IsPurchased && !o.IsRejected) // فقط الطلبات في الانتظار
                .Count();

            return Json(new { count = pendingOrdersCount });
        }

        [HttpGet]
        [Authorize(Roles = "personer")] // ✅ السماح فقط للمسؤول
        public async Task<IActionResult> Orders()
        {
            var orders = await _db.UTS
                .Include(ut => ut.User) // ✅ تحميل بيانات المستخدم
                .Include(ut => ut.Ticket) // ✅ تحميل بيانات التذكرة
                .ToListAsync();

            return View(orders);
        }
        [HttpPost]
        [Authorize(Roles = "personer")] // ✅ السماح فقط للمسؤول
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var order = await _db.UTS.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IsPurchased = true;
            _db.UTS.Update(order);
            await _db.SaveChangesAsync();

            return RedirectToAction("Orders");
        }

        [HttpPost]
        public IActionResult RejectOrder(int id)
        {
            var order = _db.UTS.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            // ✅ تحديث حالة الطلب ليكون مرفوضًا دون حذفه
            order.IsRejected = true;
            _db.SaveChanges();

            return RedirectToAction("Orders");
        }


        [HttpGet]
        public IActionResult Review(int id)
        {
            var order = _db.Tickets.FirstOrDefault(t => t.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }


        [HttpPost]
        public IActionResult SubmitReview(int orderId, int rating, string comment)
        {
            var order = _db.Tickets.FirstOrDefault(t => t.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Rating = rating;
            order.ReviewComment = comment;
            _db.SaveChanges();

            return RedirectToAction("ReviewDetails", new { id = orderId });
        }
        [HttpGet]
        public IActionResult ReviewDetails(int id)
        {
            var order = _db.Tickets.FirstOrDefault(t => t.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}
