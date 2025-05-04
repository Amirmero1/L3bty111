using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace My_ticket.Request
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }


}
