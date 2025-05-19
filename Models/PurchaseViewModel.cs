using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class PurchaseViewModel
    {
        public int PurchaseId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Price { get; set; }

        public int BookId { get; set; }
    }
}