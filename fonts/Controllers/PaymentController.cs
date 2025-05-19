using eBookLibrary3.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBookLibrary3.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayPalPaymentService _payPalPaymentService = new PayPalPaymentService();

        public ActionResult MakePayment(decimal amount)
        {
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            var returnUrl = baseUrl + "/Payment/Success";
            var cancelUrl = baseUrl + "/Payment/Cancel";

            var payment = _payPalPaymentService.CreatePayment(returnUrl, cancelUrl, amount);

            var approvalUrl = payment.links.FirstOrDefault(x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

            return Redirect(approvalUrl);
        }

        public ActionResult Success(string paymentId, string PayerID)
        {
            var executedPayment = _payPalPaymentService.ExecutePayment(paymentId, PayerID);

            if (executedPayment.state.ToLower() != "approved")
            {
                return RedirectToAction("Checkout", "Cart");
            }

            else
            {
                using (var db = new LibraryContext())
                {
                    // קבלת המשתמש המחובר והעגלה
                    int userId = (int)Session["UserId"];
                    string userName = (string)Session["UserName"];
                    var cart = CartHelper.GetCart();

                    // חישוב הסכום הכולל
                    decimal totalAmount = cart.Sum(item => item.PurchaseType == "Buy" ? item.PurchasePrice : (item.BorrowPrice ?? 0));

                    // הוספת הזמנה לטבלת Orders
                    var order = new Orders
                    {

                        UserId = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount,
                        PaymentStatus = "Success"
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();

                    var payment = new Payments
                    {

                        UserId = userId,
                        PaymentDate = DateTime.Now,
                        Amount = totalAmount,
                        PaymentMethod = "PayPal"
                    };
                    db.Payments.Add(payment);
                    db.SaveChanges();

                    foreach (var item in cart)
                    {
                        if (item.PurchaseType == "Buy")
                        {
                            BuyBook(item.BookId); // קריאה לפונקציית הקנייה
                        }
                        else if (item.PurchaseType == "Borrow")
                        {
                            BorrowBook(item.BookId); // קריאה לפונקציית ההשאלה
                        }
                    }

                    //Session["Cart"] = null;
                    // ניקוי העגלה הנוכחית
                    CartHelper.ClearCart();

                    // שחזור העגלה הזמנית
                    CartHelper.RestoreCartFromTemporary();
                    TempData["Success"] = "The order was successfully placed";

                    // Prepare email content
                    var emailService = new EmailService(
                         "smtp.gmail.com", // SMTP server
                         587,              // SMTP port
                         "royboker15@gmail.com",  // Sender email
                         "dhue jxxw dhbd wdpl"    // Email password or app password
                    );

                    string recipientEmail = (string)Session["Email"];
                    string emailSubject = $"Order Confirmation #{order.OrderId}";
                    string emailBody = $@"
                          <p>Dear {userName},</p>
                          <p>Thank you for your order.</p>
                          <p><strong>Total Amount Paid:</strong> ${totalAmount:F2}</p>
                          <p><strong>Order Date:</strong> {order.OrderDate:MMMM dd, yyyy}</p>
                          <p><strong>Payment Method:</strong> PayPal</p>
                          <p>We hope you enjoy our services!</p>
                         <br>
                         <p>Best regards,<br>ebookLibrary Team</p>
                                ";

                    // Send the email
                    emailService.SendEmail(
                        recipientEmail,   // Recipient email
                        emailSubject,     // Email subject
                        emailBody,        // Email body
                        true              // Is HTML
                    );
                    return RedirectToAction("ShowHomePage", "HomePage");

                }
            }
        }

        public void BuyBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
               

                int userId = (int)Session["UserId"]; // קבלת מזהה המשתמש המחובר
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);


                // יצירת רכישה חדשה
                var purchase = new Purchase
                {
                    BookId = book.BookId,
                    UserId = userId,
                    PurchaseDate = DateTime.Now
                };

                // עדכון מסד הנתונים
                db.Purchases.Add(purchase);


                //עדכון פופולריות
                if (book.Popularity >= 0 && book.Popularity <= 4.5)
                {
                    book.Popularity += 0.5f;
                }
         

                db.SaveChanges();
            }
        }

        public void BorrowBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
                
                int userId = (int)Session["UserId"];

                var book = db.Books.Find(bookId);

                // הוספת השאלה לטבלה
                var borrow = new Borrow
                {
                    UserId = userId,
                    BookId = bookId,
                    BorrowDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(30)
                };
                db.Borrows.Add(borrow);

                // עדכון המלאי
                book.Stock -= 1;

                //עדכון פופולריות
                if (book.Popularity >= 0 && book.Popularity <= 4.5)
                {
                    book.Popularity += 0.2f;
                }


                db.SaveChanges();

            }
        }
    }
}