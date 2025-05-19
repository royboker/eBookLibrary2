using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; } // מוגדר כמפתח ראשי יחיד

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

    }
}