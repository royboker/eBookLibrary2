using eBookLibrary3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBookLibrary3.Controllers
{
    public class HomePageController : Controller
    {
        public ActionResult ShowHomePage()
        {
            ManageBorrows();

            using (var db = new LibraryContext())
            {
                var books = db.Books.AsQueryable();

                foreach (var book in books)
                {
                    if (book.DiscountStartDate.HasValue)
                    {
                        if ((DateTime.Now - book.DiscountStartDate.Value).TotalDays >= 7)
                        {
                            book.BuyPrice = book.Price;
                            book.DiscountStartDate = null;
                        }
                    }
                }
                db.SaveChanges();

                // שליפת הספרים הפופולריים
                var popularBooks = db.Books
                    .OrderByDescending(b => b.Popularity)
                    .Take(6) // מספר הספרים לסרט הנע
                    .ToList();



                return View(popularBooks);
            }


        }

        public ActionResult About()
        {
            using (var db = new LibraryContext())
            {
                // חישוב מספר המשתמשים והספרים
                var totalUsers = db.Users.Count();
                var totalBooks = db.Books.Count();
                var totalReviews = db.SiteReviews.Count();
                

                // שליחת המידע לדף ה-About
                ViewBag.TotalUsers = totalUsers;
                ViewBag.TotalBooks = totalBooks;
                ViewBag.TotalReviews = totalReviews;
                

                ViewBag.LatestReviews = db.SiteReviews
                          .OrderByDescending(r => r.Rating)
                          .ThenByDescending(r => r.CreatedAt) // למקרה של דירוגים שווים, נמיין לפי תאריך יצירה
                          .Take(10)
                          .ToList();
            
            }

            return View();
        }


        //public void ManageBorrows()
        //{
        //    using (var db = new LibraryContext())
        //    {
        //        // שלב 1: מחיקת השאלות שפג תוקפן
        //        var expiredBorrows = db.Borrows
        //             .Include("Book") // השתמש בשם הניווט של ה- Entity
        //             .Where(b => b.ReturnDate < DateTime.Now)
        //             .ToList();


        //        if (!expiredBorrows.Any())
        //        {
        //            return; // אם אין השאלות שפג תוקפן, אין צורך להמשיך
        //        }

        //        foreach (var borrow in expiredBorrows)
        //        {
        //            var book = borrow.Book;

        //            if (book != null)
        //            {
        //                book.Stock += 1; // הגדלת המלאי עבור הספר
        //            }

        //            db.Borrows.Remove(borrow); // מחיקת השורה מהטבלה
        //        }

        //        db.SaveChanges(); // שמירת השינויים (מחיקת השאלות + עדכון המלאי)


        //        var booksWithWaitingList = db.WaitingLists
        //                .GroupBy(w => w.BookId)
        //                .Where(g => g.Count() > 0)
        //                 .Select(g => new
        //                    {
        //                 BookId = g.Key,
        //                 WaitingList = g.OrderBy(w => w.Position).Take(3).ToList()
        //                      })
        //                     .ToList();


        //        foreach (var entry in booksWithWaitingList)
        //        {
        //            var book = db.Books.AsNoTracking().FirstOrDefault(b => b.BookId == entry.BookId);

        //            if (book != null && book.Stock > 0)
        //            {
        //                foreach (var waitingEntry in entry.WaitingList)
        //                {
        //                    var user = db.Users.AsNoTracking().FirstOrDefault(u => u.UserId == waitingEntry.UserId);
        //                    if (user != null)
        //                    {
        //                        // שליחת מייל
        //                        var emailService = new EmailService("smtp.gmail.com", 587, "royboker15@gmail.com", "dhue jxxw dhbd wdpl");
        //                        string subject = $"The book '{book.Title}' is now available!";
        //                        string body = $"Hello {user.FirstName},\n\nThe book '{book.Title}' is now available for you to borrow. Please log in to your account to complete the borrowing process.";
        //                        emailService.SendEmail(user.Email, subject, body, true);

        //                        // עדכון הרשאה למשתמש
        //                        //waitingEntry.AllowedToBorrow = true; // אם שדה זה קיים
        //                    }
        //                }
        //            }
        //        }

        //        db.SaveChanges(); // שמירת השינויים (הרשאות שנוספו)
        //    }
        //}

        //public void ManageBorrows()
        //{
        //    using (var db = new LibraryContext())
        //    {

        //        //  טיפול ברשימת ההמתנה (הסרת משתמשים שחלף חודש)
        //        var expiredWaitingListEntries = db.WaitingLists
        //            .Where(w => DbFunctions.DiffDays(w.AddedAt, DateTime.Now) > 30) // בדיקת משתמשים שחלף חודש מאז שהצטרפו לרשימת המתנה
        //            .ToList();

        //        foreach (var entry in expiredWaitingListEntries)
        //        {
        //            db.WaitingLists.Remove(entry); // הסרת המשתמש מרשימת ההמתנה

        //            // עדכון פוזיציות של שאר המשתמשים ברשימה
        //            var waitingList = db.WaitingLists
        //                .Where(w => w.BookId == entry.BookId && w.Position > entry.Position)
        //                .OrderBy(w => w.Position)
        //                .ToList();

        //            foreach (var waitingUser in waitingList)
        //            {
        //                waitingUser.Position--; // קידום הפוזיציה
        //            }
        //        }

        //        db.SaveChanges();


        //        // שלב 1: מחיקת השאלות שפג תוקפן
        //        var expiredBorrows = db.Borrows
        //             .Include("Book") // השתמש בשם הניווט של ה- Entity
        //             .Where(b => b.ReturnDate < DateTime.Now)
        //             .ToList();

        //        if (!expiredBorrows.Any())
        //        {
        //            return; // אם אין השאלות שפג תוקפן, אין צורך להמשיך
        //        }

        //        foreach (var borrow in expiredBorrows)
        //        {
        //            var book = borrow.Book;

        //            if (book != null)
        //            {
        //                book.Stock += 1; // הגדלת המלאי עבור הספר
        //            }

        //            db.Borrows.Remove(borrow); // מחיקת השורה מהטבלה
        //        }

        //        db.SaveChanges(); // שמירת השינויים (מחיקת השאלות + עדכון המלאי)

        //         // שמירת השינויים (הסרת משתמשים ועדכון פוזיציות)

        //        // שלב 3: שליחת מייל לשלושת הראשונים ברשימת ההמתנה
        //        var booksWithWaitingList = db.WaitingLists
        //                .GroupBy(w => w.BookId)
        //                .Where(g => g.Count() > 0)
        //                 .Select(g => new
        //                 {
        //                     BookId = g.Key,
        //                     WaitingList = g.OrderBy(w => w.Position).Take(3).ToList()
        //                 })
        //                     .ToList();

        //        foreach (var entry in booksWithWaitingList)
        //        {
        //            var book = db.Books.AsNoTracking().FirstOrDefault(b => b.BookId == entry.BookId);

        //            if (book != null && book.Stock > 0)
        //            {
        //                foreach (var waitingEntry in entry.WaitingList)
        //                {
        //                    var user = db.Users.AsNoTracking().FirstOrDefault(u => u.UserId == waitingEntry.UserId);
        //                    if (user != null)
        //                    {
        //                        // שליחת מייל
        //                        var emailService = new EmailService("smtp.gmail.com", 587, "royboker15@gmail.com", "dhue jxxw dhbd wdpl");
        //                        string subject = $"The book '{book.Title}' is now available!";
        //                        string body = $"Hello {user.FirstName},\n\nThe book '{book.Title}' is now available for you to borrow. Please log in to your account to complete the borrowing process.";
        //                        emailService.SendEmail(user.Email, subject, body, true);

        //                        // עדכון הרשאה למשתמש (אם רלוונטי)
        //                        // waitingEntry.AllowedToBorrow = true; // אם שדה זה קיים
        //                    }
        //                }
        //            }
        //        }

        //        db.SaveChanges(); // שמירת השינויים (הרשאות שנוספו)
        //    }
        //}


        public void ManageBorrows()
        {
            using (var db = new LibraryContext())
            {
                // שלב 1: מחיקת השאלות שפג תוקפן והגדלת המלאי
                var expiredBorrows = db.Borrows
                    .Include("Book")
                    .Where(b => b.ReturnDate < DateTime.Now)
                    .ToList();

                var updatedBooks = new HashSet<int>(); // סט של מזהי ספרים שחזרו למלאי

                foreach (var borrow in expiredBorrows)
                {
                    var book = borrow.Book;

                    if (book != null)
                    {
                        book.Stock += 1; // הגדלת המלאי עבור הספר
                        updatedBooks.Add(book.BookId); // הוספת הספר לרשימת הספרים שחזרו למלאי
                    }

                    db.Borrows.Remove(borrow); // מחיקת השורה מהטבלה
                }

                db.SaveChanges();

                // שלב 2: טיפול ברשימת ההמתנה
                // הסרת משתמשים שחלף חודש מאז שנוספו
                var expiredWaitingListEntries = db.WaitingLists
                    .Where(w => DbFunctions.DiffDays(w.AddedAt, DateTime.Now) > 30)
                    .ToList();

                foreach (var entry in expiredWaitingListEntries)
                {
                    db.WaitingLists.Remove(entry); // הסרת המשתמש מרשימת ההמתנה

                    // עדכון פוזיציות של שאר המשתמשים ברשימה
                    var waitingList = db.WaitingLists
                        .Where(w => w.BookId == entry.BookId && w.Position > entry.Position)
                        .OrderBy(w => w.Position)
                        .ToList();

                    foreach (var waitingUser in waitingList)
                    {
                        waitingUser.Position--; // קידום הפוזיציה
                    }
                }

                db.SaveChanges();

                // שלב 3: שליחת מייל לשלושת הראשונים ברשימת ההמתנה עבור ספרים שחזרו למלאי בלבד
                foreach (var bookId in updatedBooks)
                {
                    var book = db.Books.AsNoTracking().FirstOrDefault(b => b.BookId == bookId);

                    if (book != null && book.Stock > 0)
                    {
                        var waitingList = db.WaitingLists
                            .Where(w => w.BookId == bookId)
                            .OrderBy(w => w.Position)
                            .Take(3)
                            .ToList();

                        foreach (var waitingEntry in waitingList)
                        {
                            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.UserId == waitingEntry.UserId);
                            if (user != null)
                            {
                                // שליחת מייל
                                var emailService = new EmailService("smtp.gmail.com", 587, "royboker15@gmail.com", "dhue jxxw dhbd wdpl");
                                string subject = $"The book '{book.Title}' is now available!";
                                string body = $"Hello {user.FirstName},\n\nThe book '{book.Title}' is now available for you to borrow. Please log in to your account to complete the borrowing process.";
                                emailService.SendEmail(user.Email, subject, body, true);

                                // עדכון הרשאה למשתמש (אם רלוונטי)
                                // waitingEntry.AllowedToBorrow = true; // אם שדה זה קיים
                            }
                        }
                    }
                }

                db.SaveChanges(); // שמירת השינויים
            }
        }

    }
}