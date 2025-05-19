using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class RateBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public float Rating { get; set; }
    }
}