using System.ComponentModel.DataAnnotations;

namespace My_ticket.Request
{
    public class CompleteProfileViewModel
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "يجب أن تحتوي كلمة المرور على 6 أحرف على الأقل")]
        public string Password { get; set; }

        public CompleteProfileViewModel() { }

        public CompleteProfileViewModel(string email, string phone, string address)
        {
            Email = email;
            Phone = phone;
            Address = address;
        }
    }
}
