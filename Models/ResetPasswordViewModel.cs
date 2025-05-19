using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; } // טוקן שיאמת את המשתמש

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } // הסיסמה החדשה

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } // אימות הסיסמה החדשה
    }
}