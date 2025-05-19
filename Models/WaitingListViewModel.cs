using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class WaitingListViewModel
    {
        public string UserName { get; set; } // שם המשתמש
        public int Position { get; set; } // מיקום בתור

        public DateTime EstimatedAvailability { get; set; }
    }
}