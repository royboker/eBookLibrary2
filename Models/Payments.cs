using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class Payments
    {
        [Key] 
        public int PaymentId { get; set; }
        

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required, StringLength(50)]
        public string PaymentMethod { get; set; }

        public int UserId { get; set; } // מפתח זר לקשר עם משתמש
        public virtual User User { get; set; }
    }
}