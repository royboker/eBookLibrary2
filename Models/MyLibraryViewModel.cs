using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class MyLibraryViewModel
    {
        public IEnumerable<PurchaseViewModel> PurchasedBooks { get; set; }
        public IEnumerable<Borrow> BorrowedBooks { get; set; }
    }
}