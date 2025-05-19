using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace eBookLibrary3.Models
{
    public class LibraryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Payments> Payments { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<SiteReview> SiteReviews { get; set; }
        public DbSet<WaitingList> WaitingLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}