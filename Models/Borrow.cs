using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class Borrow
    {
        [Key]
        public int BorrowId { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        public DateTime ReturnDate { get; set; }
    }
}