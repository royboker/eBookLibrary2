using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class PaymentModel
    {
        [Required(ErrorMessage = "Card holder name is required.")]
        public string CardHolderName { get; set; }

        [Required(ErrorMessage = "Card number is required.")]
        [CreditCard(ErrorMessage = "Invalid card number.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry date is required.")]
        [RegularExpression(@"\d{2}/\d{2}", ErrorMessage = "Expiry date must be in the format MM/YY.")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be 3 digits.")]
        public string CVV { get; set; }
    }
}