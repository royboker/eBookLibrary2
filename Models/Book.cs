using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        [StringLength(100)]
        public string Publisher { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        // שדה URL לתמונה
        [StringLength(1000)]
        public string ImageUrl { get; set; }

        // מחיר קנייה
        public decimal BuyPrice { get; set; }

        // מחיר השאלה
        public decimal BorrowPrice { get; set; }

        // שנה של פרסום
        public int YearOfPublication { get; set; }

        // גיל מינימלי לקריאה
        public string AgeLimit { get; set; }

        // ז'אנר הספר
        [StringLength(100)]
        public string Genre { get; set; }

        // פורמט הספר
        [StringLength(100)]
        public string Format { get; set; }

        public float Popularity { get; set; } = 0.0f; // ברירת מחדל: 0
        public int ReviewCount { get; set; } = 0; // ברירת מחדל: 0

        [StringLength(1000)]
        public string Summary { get; set; }

        public DateTime? DiscountStartDate { get; set; }
    }
}