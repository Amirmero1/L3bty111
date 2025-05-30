﻿using System.ComponentModel.DataAnnotations;

namespace My_ticket.Request
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } 

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
