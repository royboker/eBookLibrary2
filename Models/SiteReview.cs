using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class SiteReview
    {
        [Key]
        public int ReviewId { get; set; }
        public int UserId { get; set; } 
        public virtual User User { get; set; }                            
        public string UserName { get; set; } // שם המשתמש
        public string Content { get; set; } // תוכן הביקורת
        public float Rating { get; set; } // ציון בין 0 ל-5
        public DateTime CreatedAt { get; set; }
    }
}