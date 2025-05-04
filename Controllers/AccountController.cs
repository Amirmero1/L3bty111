using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using My_ticket.Data;
using My_ticket.Models;
using My_ticket.Request;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Authentication.Google;

namespace My_ticket.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = $"{nameof(Role.personer)}")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Resgisterperson()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Resgisterperson(CreatepersonRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var foundperson = _db.user1.FirstOrDefault(x => x.Email == request.Email);
            if (foundperson != null)
            {
                ModelState.AddModelError("Email", "There is already a person with the same email.");
                return View(request);
            }

            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                Password = request.Password,
                Phone = request.Phone,
                Address = request.Address,
                Role = Role.person,
                 Rating = 5,
                ReviewComment = "لا يوجد",
            };

            _db.user1.Add(user);
            _db.SaveChanges();
            return RedirectToAction("Index", "Tickets");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var user = _db.user1.FirstOrDefault(x => x.Password == request.Password && x.Email == request.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password is wrong. Please try again!");
                return View(request);
            }

            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Sid, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("FullName", user.FullName),
        new Claim(ClaimTypes.StreetAddress, user.Address),
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // ✅ إعداد خيارات الكوكيز
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = request.RememberMe,
                ExpiresUtc = request.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            Console.WriteLine($"✅ Login Successful: {user.FullName} - {user.Email}");

            return RedirectToAction("Index", "Tickets");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult GoogleLogin(bool isRegister = false)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { isRegister });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse(bool isRegister = false)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
                return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            });

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "تعذر استرداد البريد الإلكتروني من جوجل!";
                return RedirectToAction("Login");
            }

            var user = _db.user1.FirstOrDefault(u => u.Email == email);

            if (user == null && isRegister)
            {
                // ✅ المستخدم في وضع التسجيل، نقوم بإنشاء حساب جديد
                user = new User
                {
                    Email = email,
                    FullName = name ?? "Unknown",
                    Password = "GoogleAccount",
                    Phone = "غير محدد",
                    Address = "غير محدد",
                    Role = Role.person,
                    Rating = 5,
                    ReviewComment = "لا يوجد"
                };

                _db.user1.Add(user);
                _db.SaveChanges();

                return RedirectToAction("CompleteProfile", "Account", new { email = user.Email });
            }
            else if (user == null)
            {
                // ❌ المستخدم حاول تسجيل الدخول بحساب غير مسجل
                HttpContext.Session.SetString("ErrorMessage", "لا يوجد حساب مرتبط بهذا البريد الإلكتروني، يرجى إنشاء حساب أولاً.");
                return RedirectToAction("Login");
            }

            // ✅ تسجيل الدخول للمستخدم الموجود
            List<Claim> userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Sid, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("FullName", user.FullName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Tickets");
        }


        [HttpGet]
        public IActionResult CompleteProfile(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var user = _db.user1.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new CompleteProfileViewModel
            {
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteProfile(CompleteProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _db.user1.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Address) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "يجب إدخال جميع البيانات المطلوبة!");
                return View(model);
            }

            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Password = model.Password; 
            _db.SaveChanges();

            return RedirectToAction("Index", "Tickets");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _db.user1.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["Error"] = "البريد الإلكتروني غير موجود.";
                return RedirectToAction("ForgotPassword");
            }

            // توليد كود تحقق 6 أرقام
            var code = new Random().Next(100000, 999999).ToString();

            // خزنه في الجلسة
            HttpContext.Session.SetString("ResetCode", code);
            HttpContext.Session.SetString("ResetEmail", email);

            // إرسال الكود على الإيميل
            try
            {
                await new EmailService().SendEmailAsync(email, "رمز التحقق", $"رمز التحقق الخاص بك هو: {code}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إرسال رمز التحقق: {ex.Message}";
                return RedirectToAction("ForgotPassword");
            }

            return View("ForgotPasswordConfirmation"); // صفحة جديدة يكتب فيها الكود
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult EnterCode(string code)
        {
            var sessionCode = HttpContext.Session.GetString("ResetCode");

            if (sessionCode == null || code != sessionCode)
            {
                TempData["Error"] = "رمز التحقق غير صحيح.";
                return View("ForgotPasswordConfirmation");
            }

            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = HttpContext.Session.GetString("ResetEmail");
            var user = _db.user1.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["Error"] = "حدث خطأ ما.";
                return RedirectToAction("Login");
            }

            user.Password = model.Password;
            _db.SaveChanges();

            TempData["Success"] = "تم تغيير كلمة المرور بنجاح!";
            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("ResetCode");

            return RedirectToAction("Login");
        }


    }
}
