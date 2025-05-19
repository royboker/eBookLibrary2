using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Required, StringLength(200)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public virtual ICollection<Borrow> Borrows { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }

        public virtual ICollection<Payments> Payments { get; set; }

        public string PasswordResetToken { get; set; }
        public DateTime? TokenExpiryTime { get; set; }
    }
}