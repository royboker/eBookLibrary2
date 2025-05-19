using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; } // מפתח ראשי

        public int UserId { get; set; } // מפתח זר לקשר עם משתמש
        public virtual User User { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } // תאריך ההזמנה

        [Required]
        public decimal TotalAmount { get; set; } // סה"כ מחיר

        [StringLength(50)]
        public string PaymentStatus { get; set; }
    }
}