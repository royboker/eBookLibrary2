using eBookLibrary3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace eBookLibrary3.Controllers
{
    public class AccountController : Controller
    {
        // פעולה לדף ההרשמה
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult SubmitAccount(User model)
        {

            if (ModelState.IsValid)
            {
                using (var db = new LibraryContext())
                {
                    // בדיקת האם המייל כבר קיים
                    var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                    if (existingUser != null)
                    {
                        ViewBag.ErrorMessage = "Email already exists.";
                        return View(model);
                    }
                    model.PasswordHash = PasswordHelper.HashPassword(model.PasswordHash);
                    model.RoleId = 2;//ברירת מחדל עובר משתמשים רגילים
                    // הוספת המשתמש למסד הנתונים
                    db.Users.Add(model);
                    db.SaveChanges();
                }

                // הפניה לדף ההתחברות לאחר הרשמה מוצלחת
                return RedirectToAction("Login", "Account");
            }

            // חזרה לדף ההרשמה במקרה של שגיאה בנתונים
            return View("Register", model);
        }

        // פעולה לדף ההתחברות
        public ActionResult Login()
        {
            return View();
        }

        // פעולה לדף ההתחברות
        public ActionResult SubmitLogin(string Email, string PasswordHash)
        {
            using (var db = new LibraryContext())
            {
                // מצא את המשתמש לפי המייל
                var user = db.Users.FirstOrDefault(u => u.Email == Email);
                if (user == null)
                {
                    ModelState.AddModelError("LoginError", "User not found.");
                    return View("Login");
                }

                // הצפן את הסיסמה שהוזנה והשווה
                string hashedPassword = PasswordHelper.HashPassword(PasswordHash);
                if (user.PasswordHash != hashedPassword)
                {
                    ModelState.AddModelError("LoginError", "Invalid password.");
                    return View("Login");
                }

                // התחברות הצליחה
                Session["UserId"] = user.UserId; // שמירת מזהה המשתמש בסשן
                Session["UserName"] = user.FirstName;
                Session["Role"] = user.RoleId;
                Session["Email"] = user.Email;
                return RedirectToAction("Dashboard", "Account");
            }
        }


        public ActionResult Dashboard()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // בדיקת תפקיד המשתמש
            int roleId = (int)Session["Role"];
            if (roleId == 2) // 2 = משתמש רגיל
            {
                return View("UserDashboard");
            }
            else if (roleId == 1) // 1 = Admin
            {
                return View("AdminDashboard");
            }

            // במקרה של תפקיד לא מוכר
            TempData["ErrorMessage"] = "Unauthorized access.";
            return RedirectToAction("Login", "Account");
        }

        private bool IsUserLoggedIn()
        {
            return Session["UserId"] != null;
        }

        [HttpGet]
        public ActionResult UpdateDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new LibraryContext())
            {
                var userId = (int)Session["UserId"];
                var user = db.Users.Find(userId);
                return View(user);
            }
        }

        [HttpPost]
        public ActionResult UpdateDetails(User model)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                using (var db = new LibraryContext())
                {
                    var user = db.Users.Find(model.UserId);
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;
                        user.PasswordHash = PasswordHelper.HashPassword(model.PasswordHash);

                        db.SaveChanges();
                    }
                }


                TempData["Message"] = "Details updated successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(model);
        }
        public ActionResult Logout()
        {
            // נקה את ה-Session
            Session.Clear();
            Session.Abandon();

            // הפנה לדף הבית או לדף התחברות
            return RedirectToAction("ShowHomePage", "HomePage");
        }

        public ActionResult UserPurchases()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to view your purchases.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new LibraryContext())
            {
                var userId = (int)Session["UserId"];
                var purchases = db.Purchases
                                  .Where(p => p.UserId == userId)
                                  .Include(p => p.Book) // לוודא שה-Book נכלל ב-Query
                                  .Select(p => new PurchaseViewModel
                                  {
                                      PurchaseId = p.PurchaseId,
                                      Title = p.Book.Title,
                                      Author = p.Book.Author,
                                      ImageUrl = p.Book.ImageUrl,
                                      PurchaseDate = p.PurchaseDate,
                                      Price = p.Book.BuyPrice,
                                      BookId = p.Book.BookId,
                                  })
                                  .ToList();

                return View(purchases);
            }
        }


        public ActionResult MyBorrows()
        {
            int userId = (int)Session["UserId"];
            using (var db = new LibraryContext())
            {
                var borrows = db.Borrows
                    .Where(b => b.UserId == userId && b.ReturnDate > DateTime.Now)
                    .Include(b => b.Book)
                    .ToList();

                return View(borrows);
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email, string firstName)
        {
            using (var db = new LibraryContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email && u.FirstName == firstName);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found with the provided details.");
                    return View();
                }

                // שמירת מזהה המשתמש ב-Session כדי לשימוש בדף הבא
                Session["ResetPasswordUserId"] = user.UserId;

                // הפניה לדף הזנת סיסמא חדשה
                return RedirectToAction("ResetPassword");
            }
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            // וידוא שהמשתמש הגיע לדף זה דרך התהליך הנכון
            if (Session["ResetPasswordUserId"] == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (Session["ResetPasswordUserId"] == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Message"] = "Passwords do not match.";
                return View();
            }

            using (var db = new LibraryContext())
            {
                int userId = (int)Session["ResetPasswordUserId"];
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                {
                    return HttpNotFound();
                }

                // עדכון הסיסמא (רצוי להוסיף הצפנה לסיסמא)
                user.PasswordHash = PasswordHelper.HashPassword(newPassword);
                // מחליף את פונקציית ההצפנה שלך
                db.SaveChanges();
            }

            // ניקוי ה-Session
            Session["ResetPasswordUserId"] = null;

            TempData["Message"] = "Your password has been updated successfully.";
            return RedirectToAction("Login");
        }

        public ActionResult UserOrders()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to view your orders.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new LibraryContext())
            {
                int userId = (int)Session["UserId"];
                var orders = db.Orders
                               .Where(o => o.UserId == userId)
                               .ToList();

                var totalOrders = orders.Count;
                var totalAmount = orders.Sum(o => o.TotalAmount);

                ViewBag.TotalOrders = totalOrders;
                ViewBag.TotalAmount = totalAmount;

                return View(orders);
            }
        }

        public ActionResult UserPayments()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to view your payments.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new LibraryContext())
            {
                int userId = (int)Session["UserId"];
                var payments = db.Payments
                                 .Where(p => p.UserId == userId)
                                 .ToList(); // מחזיר רשימת Payments במקום אובייקטים אנונימיים

                var totalPayments = payments.Count;
                var totalAmountPaid = payments.Sum(p => p.Amount);

                ViewBag.TotalPayments = totalPayments;
                ViewBag.TotalAmountPaid = totalAmountPaid;

                return View(payments);
            }
        }

        public ActionResult MyLibrary()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to access your library.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new LibraryContext())
            {
                var userId = (int)Session["UserId"];

                // ספרים שנרכשו
                var purchasedBooks = db.Purchases
                    .Where(p => p.UserId == userId)
                    .Include(p => p.Book)
                    .Select(p => new PurchaseViewModel
                    {
                        PurchaseId = p.PurchaseId,
                        Title = p.Book.Title,
                        Author = p.Book.Author,
                        ImageUrl = p.Book.ImageUrl,
                        PurchaseDate = p.PurchaseDate,
                        Price = p.Book.BuyPrice,
                        BookId = p.Book.BookId,
                    })
                    .ToList();

                // ספרים מושאלים
                var borrowedBooks = db.Borrows
                    .Where(b => b.UserId == userId && b.ReturnDate > DateTime.Now)
                    .Include(b => b.Book)
                    .ToList();

                var model = new MyLibraryViewModel
                {
                    PurchasedBooks = purchasedBooks,
                    BorrowedBooks = borrowedBooks
                };

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult AddReview()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to log in to add a review.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddReview(string content, float rating)
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to log in to add a review.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new LibraryContext())
            {
                int userId = (int)Session["UserId"];
                string userName = (string)Session["UserName"];

                if (rating < 0 || rating > 5)
                {
                    TempData["Message"] = "Rating must be between 0 and 5.";
                    return RedirectToAction("AddReview");
                }

                var review = new SiteReview
                {
                    UserId = userId,
                    UserName = userName,
                    Content = content,
                    Rating = rating,
                    CreatedAt = DateTime.Now
                };

                db.SiteReviews.Add(review);
                db.SaveChanges();

                TempData["Message"] = "Your review was added successfully!";
                return RedirectToAction("About", "HomePage");
            }
        }

    }
}