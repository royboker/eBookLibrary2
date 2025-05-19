using eBookLibrary3.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBookLibrary3.Controllers
{
    public class CartController : Controller
    {
        public ActionResult ShowCart()
        {
            var cart = CartHelper.GetCart();
            return View(cart);
        }
        [HttpPost]
        public ActionResult AddToCart(int bookId, string type)
        {
            
            using (var db = new LibraryContext())
            {
                var book = db.Books.Find(bookId);
                if (book != null)
                {
                    decimal price = 0;
                    if (book.BuyPrice == 0)
                    {
                        book.BuyPrice = book.Price;
                    }
                    if (type == "Buy")
                    {
                        price = book.BuyPrice;
                    }
                   

                    else if (type == "Borrow" && book.BorrowPrice != 0)
                    {
                        if (Session["UserId"] == null)
                        {
                            TempData["ErrorMessage"] = "You need to log in to borrow a book.";
                            return RedirectToAction("Login", "Account");
                        }
                        int userId = (int)Session["UserId"];

                        // בדיקה אם למשתמש יש כבר 3 ספרים מושאלים
                        var activeBorrows = db.Borrows.Count(b => b.UserId == userId && b.ReturnDate > DateTime.Now);
                        if (activeBorrows >= 3)
                        {
                            TempData["ErrorMessage"] = "You cannot borrow more than 3 books at the same time.";
                            return RedirectToAction("ShowLibrary", "Library");
                        }

                        // בדיקת רשימת המתנה
                        var waitingList = db.WaitingLists
                            .Where(w => w.BookId == bookId)
                            .OrderBy(w => w.Position)
                            .ToList();

                        if (waitingList.Count >= 3) // יש רשימת המתנה של 3 אנשים או יותר
                        {
                            var isUserInTopThree = waitingList
                                .Take(3) // בודקים רק את שלושת הראשונים
                                .Any(w => w.UserId == userId);

                            if (!isUserInTopThree) // המשתמש לא נמצא בשלושת הראשונים
                            {
                                TempData["ErrorMessage"] = "You cannot borrow this book because there is a waiting list, and you are not among the top three in the list.";
                                return RedirectToAction("ShowLibrary", "Library");
                            }
                        }

                        // אם יש פחות מ-3 אנשים ברשימת ההמתנה, מותר להשאיל
                        price = book.BorrowPrice;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The selected purchase type is not available for this book.";
                        return RedirectToAction("ShowCart");
                    }

                    var cartItem = new CartItem
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        PurchasePrice = book.BuyPrice,
                        BorrowPrice = book.BorrowPrice,
                        PurchaseType = type,
                        ImageUrl = book.ImageUrl,
                        ItemStock= book.Stock,
                    };

                    CartHelper.AddToCart(cartItem);
                }
            }
            return RedirectToAction("ShowCart");
        }

        
        public ActionResult BuyNow(int bookId, string type)
        {
            using (var db = new LibraryContext())
            {
                var book = db.Books.Find(bookId);
                if (book == null)
                {
                    TempData["ErrorMessage"] = "The selected book does not exist.";
                    return RedirectToAction("ShowCart");
                }

                // שמירת העגלה הנוכחית כעגלה זמנית
                CartHelper.SaveCartToTemporary();

                // ניקוי העגלה הקיימת
                CartHelper.ClearCart();

                // יצירת פריט חדש לעגלה עבור הספר הרצוי
                var cartItem = new CartItem
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    PurchasePrice = book.BuyPrice,
                    BorrowPrice = book.BorrowPrice,
                    PurchaseType = type,
                    ImageUrl = book.ImageUrl
                };

                // הוספת הספר הנבחר לעגלה
                CartHelper.AddToCart(cartItem);

                // מעבר לשלב Checkout
                return RedirectToAction("Checkout", "Cart");
            }
        }



        [HttpPost]
        public ActionResult RemoveFromCart(int bookId)
        {
            CartHelper.RemoveFromCart(bookId);
            return RedirectToAction("ShowCart");
        }

        [HttpPost]
        public ActionResult ClearCart()
        {
            CartHelper.ClearCart(); // פעולה שמנקה את עגלת הקניות
            TempData["SuccessMessage"] = "Cart has been cleared.";
            return RedirectToAction("ShowCart");
        }

        [HttpPost]
        public ActionResult UpdatePurchaseType(int bookId, string purchaseType)
        {
            var cart = CartHelper.GetCart();

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.BookId == bookId);
                if (item != null)
                {
                    if (purchaseType == "Borrow")
                    {
                        if (item.BorrowPrice.HasValue)
                        {
                            item.PurchaseType = "Borrow";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "This book cannot be borrowed.";
                        }
                    }
                    else if (purchaseType == "Buy")
                    {
                        item.PurchaseType = "Buy";
                    }
                }
            }

            return RedirectToAction("ShowCart");
        }


        
        public ActionResult Checkout()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to log in to proceed to checkout.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Cart") });
            }

            int userId = (int)Session["UserId"];
            var cart = CartHelper.GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("ShowCart");
            }

            using (var db = new LibraryContext())
            {
                var activeBorrows = db.Borrows.Count(b => b.UserId == userId && b.ReturnDate > DateTime.Now);

                var itemsToRemove = new List<CartItem>();

                foreach (var item in cart)
                {
                    if (item.PurchaseType == "Borrow")
                    {
                        if (activeBorrows >= 3)
                        {
                            itemsToRemove.Add(item); // הוספת הספר לרשימה למחיקה
                            TempData["ErrorMessage"] = $"The book '{item.Title}' was removed from your cart because you already have 3 borrowed books.";
                        }
                        else
                        {
                            activeBorrows++; // הגדלת הספירה עבור ספר מושאל חדש
                        }
                    }
                }

                // מחיקת ספרים בעגלה
                foreach (var item in itemsToRemove)
                {
                    CartHelper.RemoveFromCart(item.BookId);
                }

                if (itemsToRemove.Any())
                {
                    return RedirectToAction("ShowCart"); // החזרה לעגלה אם היו שינויים
                }
            }

            return View(cart); // מעבר לעמוד התשלום אם הכל תקין
        }


        public ActionResult CardUserPayment()
        {
            // השג את העגלה או הנתונים הרלוונטיים
            var cart = CartHelper.GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "No items in the cart.";
                return RedirectToAction("Cart", "Cart");
            }

            return View(cart);
        }

        [HttpPost]
        public ActionResult ValidatePayment(string cardHolderName, string cardNumber, string expiryDate, string CVV)
        {
            // 1. Validate CVC length
            if (string.IsNullOrWhiteSpace(CVV) || CVV.Length != 3)
            {
                TempData["Error"] = "CVC must be 3 digits.";
                return RedirectToAction("CardUserPayment"); // חזרה לדף תשלום
            }

            // 2. Validate expiry date format
            if (!DateTime.TryParseExact("01/" + expiryDate, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var expiryDateTime))
            {
                TempData["Error"] = "Invalid expiry date format. Use MM/YY.";
                return RedirectToAction("CardUserPayment");
            }

            // 3. Check if expiry date has passed
            if (expiryDateTime < DateTime.Now)
            {
                TempData["Error"] = "The card has expired.";
                return RedirectToAction("CardUserPayment");
            }
            // בדיקת אורך הכרטיס (לרוב בין 13-19 ספרות)
            if (cardNumber.Length < 13 || cardNumber.Length > 19)
            {
                TempData["Error"] = "The card number syntx is worng.";
                return RedirectToAction("CardUserPayment");
            }
            if (!cardNumber.All(char.IsDigit))
            {
                TempData["Error"] = "The card number syntx is worng.";
                return RedirectToAction("CardUserPayment");
            }

            // 4. Perform further validations (e.g., Luhn algorithm for card number)
            if (!ValidateCardNumber(cardNumber))
            {
                TempData["Error"] = "Invalid card number.";
                return RedirectToAction("CardUserPayment");
            }

            // If all validations pass, proceed with order creation
            TempData["Success"] = "Payment validated successfully!";
            return RedirectToAction("CompleteOrder");
        }

        // פונקציה לבדיקת כרטיס האשראי
        private bool ValidateCardNumber(string cardNumber)
        {
            // לוגיקת בדיקה לדוגמה (Luhn Algorithm)
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n;
                if (!int.TryParse(cardNumber[i].ToString(), out n))
                {
                    return false;
                }

                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n -= 9;
                    }
                }

                sum += n;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
        }

        public ActionResult CompleteOrder()
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
                    PaymentMethod = "CreditCard"
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
                          <p><strong>Payment Method:</strong>Credit Card</p>
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
           // return RedirectToAction("OrderSummary");
        }
        public void BuyBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
                // בדיקת אם המשתמש מחובר
                if (Session["UserId"] == null)
                {
                    TempData["ErrorMessage"] = "You need to log in to purchase this book.";
                    // return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
                }

                int userId = (int)Session["UserId"]; // קבלת מזהה המשתמש המחובר

                // בדיקת קיום הספר במסד הנתונים
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null || book.Stock <= 0)
                {
                    TempData["ErrorMessage"] = "The book is unavailable.";
                    //  return RedirectToAction("ShowHomePage", "HomePage");
                }

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
                // book.TimesBorrowed++;

                if (book.Stock == 0)
                {
                    db.Books.Remove(book); // הסרת הספר אם המלאי אזל
                }

                db.SaveChanges();

                // הודעת הצלחה
                TempData["SuccessMessage"] = $"You successfully purchased {book.Title}.";
                // return RedirectToAction("ShowHomePage", "HomePage");
            }
        }

        public void BorrowBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
                // בדיקת אם המשתמש מחובר
                if (Session["UserId"] == null)
                {
                    TempData["ErrorMessage"] = "You need to log in to purchase this book.";
                    // return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
                }
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
                    book.Popularity += 0.5f;
                }


                db.SaveChanges();

                TempData["SuccessMessage"] = "Book borrowed successfully!";
                //return RedirectToAction("ShowHomePage", "HomePage");
            }
        }

    }
}