using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class WaitingList
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int Position { get; set; }
        public DateTime AddedAt { get; set; }
    }
}