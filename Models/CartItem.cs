using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? BorrowPrice { get; set; } // Nullable if borrowing is unavailable
        public string PurchaseType { get; set; } // "Buy" or "Borrow"
        public string ImageUrl { get; set; }
        public int ItemStock { get; set; }
    }
}