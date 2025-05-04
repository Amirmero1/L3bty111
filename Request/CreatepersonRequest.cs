using System.ComponentModel.DataAnnotations;

public class CreatepersonRequest
{
    [Required]
    [MinLength(5)]
    [MaxLength(50)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required]
    [Phone(ErrorMessage = "Invalid phone number")]
    [MinLength(11, ErrorMessage = "Phone number must be exactly 11 digits")]
    [MaxLength(11, ErrorMessage = "Phone number must be exactly 11 digits")]
    public string Phone { get; set; }  // ✅ تمت إضافة الهاتف

    [Required]
    [MinLength(6)]
    public string Address { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }
}
