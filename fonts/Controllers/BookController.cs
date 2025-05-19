using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;
using System.Xml.Linq;
using eBookLibrary3.Models;

namespace eBookLibrary3.Controllers
{
    public class BookController : Controller
    {
        public ActionResult ShowBooks()
        {
            using (var db = new LibraryContext())
            {
                var books = db.Books.ToList();
                return View(books);
            }
        }

        public ActionResult BuyBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
                // בדיקת אם המשתמש מחובר
                if (Session["UserId"] == null)
                {
                    TempData["ErrorMessage"] = "You need to log in to purchase this book.";
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
                }

                int userId = (int)Session["UserId"]; // קבלת מזהה המשתמש המחובר

                // בדיקת קיום הספר במסד הנתונים
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null || book.Stock <= 0)
                {
                    TempData["ErrorMessage"] = "The book is unavailable.";
                    return RedirectToAction("ShowHomePage", "HomePage");
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
                book.Stock--;
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
                return RedirectToAction("ShowHomePage", "HomePage");
            }
        }

        public ActionResult BorrowBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
                // בדיקת אם המשתמש מחובר
                if (Session["UserId"] == null)
                {
                    TempData["ErrorMessage"] = "You need to log in to purchase this book.";
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
                }
                int userId = (int)Session["UserId"];

                // בדיקה אם למשתמש יש כבר 3 ספרים מושאלים
                var activeBorrows = db.Borrows.Count(b => b.UserId == userId && b.ReturnDate > DateTime.Now);
                if (activeBorrows >= 3)
                {
                    TempData["ErrorMessage"] = "You cannot borrow more than 3 books at the same time.";
                    return RedirectToAction("ShowHomePage", "HomePage");
                }

                // בדיקה אם הספר זמין
                var book = db.Books.Find(bookId);
                if (book.Stock <= 0)
                {
                    TempData["ErrorMessage"] = "This book is currently unavailable.";
                    return RedirectToAction("ShowHomePage", "HomePage");
                }

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
                return RedirectToAction("ShowHomePage", "HomePage");
            }
        }

        [HttpPost]
        public ActionResult RemoveBook(int purchaseId)
        {
            using (var db = new LibraryContext())
            {
                var purchase = db.Purchases.FirstOrDefault(p => p.PurchaseId == purchaseId);
                if (purchase != null)
                {
                    db.Purchases.Remove(purchase);
                    db.SaveChanges(); // שמירת השינויים
                }
            }
            return RedirectToAction("UserPurchases", "Account");

        }

        [HttpPost]

        public ActionResult RateBookForm(int bookId)
        {
            using (var db = new LibraryContext())
            {
                // שליפת פרטי הספר
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null)
                {
                    return HttpNotFound("Book not found.");
                }

                // יצירת ViewModel עם פרטי הספר
                var model = new RateBookViewModel
                {
                    BookId = book.BookId,
                    Title = book.Title
                };

                return View(model); // הצגת טופס הדירוג
            }
        }
        [HttpPost]
        public ActionResult SubmitRating(RateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RateBookForm", model); // אם יש שגיאות, חזרה לטופס עם שגיאות
            }

            using (var db = new LibraryContext())
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == model.BookId);
                if (book == null)
                {
                    return HttpNotFound("Book not found.");
                }

                // עדכון הפופולריות לפי הדירוג
                float popularityChange = 0;

                if (model.Rating == 0)
                {
                    popularityChange = -0.5f;
                }
                else if (model.Rating == 1)
                {
                    popularityChange = -0.25f;
                }
                else if (model.Rating == 2)
                {
                    popularityChange = 0f;
                }
                else if (model.Rating == 3)
                {
                    popularityChange = 0.25f;
                }
                else if (model.Rating == 4)
                {
                    popularityChange = 0.5f;
                }
                else if (model.Rating == 5)
                {
                    popularityChange = 0.75f;
                }

                book.Popularity = Math.Max(0, Math.Min(5, book.Popularity + popularityChange));
                book.ReviewCount++;

                db.SaveChanges();

                TempData["Message"] = "Rating submitted successfully!";
                return RedirectToAction("Dashboard", "Account"); // חזרה לעמוד הרכישות
            }
        }


        public ActionResult DownloadBorrowedBook(int bookId)
        {
            string filePath = Server.MapPath("~/App_Data/test.pdf");
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("The file does not exist.");
            }
            // שליחת הקובץ להורדה
            return File(filePath, "application/pdf", "BorrowedBook.pdf");
        }


        [HttpPost]
        public ActionResult JoinWaitingList(int bookId)
        {
            // בדיקת אם המשתמש מחובר
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to log in to get int the waiting list for this book.";
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.ToString() });
            }
            using (var db = new LibraryContext())
            {
                var userId = (int)Session["UserId"];

                if (db.WaitingLists.Any(w => w.BookId == bookId && w.UserId == userId))
                {
                    TempData["ErrorMessage"] = "You are already on the waiting list for this book.";
                    return RedirectToAction("ShowLibrary", "Library");
                }

                var position = db.WaitingLists.Count(w => w.BookId == bookId) + 1;

                var waitingListEntry = new WaitingList
                {
                    BookId = bookId,
                    UserId = userId,
                    Position = position,
                    AddedAt = DateTime.Now
                };

                db.WaitingLists.Add(waitingListEntry);
                db.SaveChanges();

                TempData["SuccessMessage"] = "You have been added to the waiting list.";
                return RedirectToAction("ShowLibrary", "Library");
            }
        }

    }
}