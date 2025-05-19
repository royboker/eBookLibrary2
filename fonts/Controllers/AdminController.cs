using eBookLibrary3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace eBookLibrary3.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowAdminLibrary()
        {
            using (var db = new LibraryContext())
            {

                var books = db.Books.ToList();
                foreach (var book in books)
                {
                    if (book.DiscountStartDate.HasValue)
                    {
                        if ((DateTime.Now - book.DiscountStartDate.Value).TotalDays >= 7)
                        {
                            book.BuyPrice = 0;
                            book.DiscountStartDate = null;
                        }
                    }
                }
                return View(books);
            }
        }

        [HttpGet]
        public ActionResult EditBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null)
                {
                    return HttpNotFound();
                }
                return View(book); // הצגת פרטי הספר לעריכה
            }
        }

        [HttpPost]
        public ActionResult EditBook(Book updatedBook)
        {
            using (var db = new LibraryContext())
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == updatedBook.BookId);
                if (book != null)
                {
                    // עדכון פרטי הספר
                    if (book.BuyPrice != 0)
                    {
                        book.BuyPrice = updatedBook.BuyPrice;
                        book.DiscountStartDate = DateTime.Now;
                    }
                    if(book.BuyPrice ==0)
                    {
                        book.BuyPrice = book.Price;
                    }

                    if(book.BuyPrice > book.Price)
                    {
                        TempData["Message"] = "Book sale price must be below the original price";
                        return RedirectToAction("ShowAdminLibrary");
                    }

                    if(book.BorrowPrice > book.Price || book.BorrowPrice > book.BuyPrice)
                    {
                        TempData["Message"] = "Borrow  price must be below the original price or the sale price";
                        return RedirectToAction("ShowAdminLibrary");
                    }
                    book.BorrowPrice = updatedBook.BorrowPrice;
                    book.Stock = updatedBook.Stock;
                    book.Genre = updatedBook.Genre;
                    book.Format = updatedBook.Format;
                    db.SaveChanges(); // שמירת שינויים
                }
                TempData["Message"] = "Book details updated successfully.";
                return RedirectToAction("ShowAdminLibrary");
            }
        }

        public ActionResult DeleteBook(int bookId)
        {
            using (var db = new LibraryContext())
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book != null)
                {
                    db.Books.Remove(book); // מחיקת הספר
                    db.SaveChanges();
                }
                TempData["Message"] = "Book deleted successfully.";
                return RedirectToAction("ShowAdminLibrary");
            }
        }

        public ActionResult ViewUsers()
        {

            using (var db = new LibraryContext())
            {
                var users = db.Users
                    .Include("Borrows.Book") // טוען את הספרים המושאלים
                    .Include("Purchases.Book") // טוען את הספרים שנרכשו
                    .ToList();

                return View(users);
            }



        }

        public ActionResult EditProfile()
        {
            int adminId = (int)Session["UserId"]; // מזהה האדמין המחובר
            using (var db = new LibraryContext())
            {
                var admin = db.Users.FirstOrDefault(u => u.UserId == adminId);
                if (admin == null)
                {
                    return HttpNotFound();
                }
                return View(admin);
            }
        }

        [HttpPost]
        public ActionResult EditProfile(User updatedAdmin)
        {
            using (var db = new LibraryContext())
            {
                var admin = db.Users.FirstOrDefault(u => u.UserId == updatedAdmin.UserId);
                if (admin != null)
                {
                    admin.FirstName = updatedAdmin.FirstName;
                    admin.Email = updatedAdmin.Email;
                    admin.PasswordHash = updatedAdmin.PasswordHash; // אם מתאפשר
                    db.SaveChanges();
                }
                return RedirectToAction("AdminDashboard");
            }
        }
        [HttpGet]
        public ActionResult AddBook()
        {
            // מציג טופס ריק להוספת ספר
            return View(new Book());
        }

        [HttpPost]
        public ActionResult AddBook(Book newBook)
        {
            if (ModelState.IsValid) // בדיקת תקינות הנתונים
            {
                if (newBook.BuyPrice > 0)
                {
                    newBook.DiscountStartDate = DateTime.Now;
                }
                else if (newBook.BuyPrice == 0)
                {
                    newBook.BuyPrice = newBook.Price;
                }
                using (var db = new LibraryContext())
                {
                    newBook.Stock = 3;
                    newBook.Popularity = 0;
                    newBook.ReviewCount = 0;
                    db.Books.Add(newBook); // הוספת הספר למסד הנתונים
                    db.SaveChanges(); // שמירת השינויים
                }
                TempData["Message"] = "Book added successfully.";
                return RedirectToAction("ShowAdminLibrary"); // חזרה לעמוד הספרייה
            }

            // אם יש שגיאות בטופס, להציג את הטופס שוב עם הודעות השגיאה
            return View(newBook);
        }

        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            using (var db = new LibraryContext())
            {
                // מציאת המשתמש לפי ID
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    // טעינת ההשאלות והרכישות של המשתמש
                    var borrows = db.Borrows.Where(b => b.UserId == userId).ToList();
                    var purchases = db.Purchases.Where(p => p.UserId == userId).ToList();

                    // מחיקת ההשאלות והרכישות
                    foreach (var borrow in borrows)
                    {
                        db.Borrows.Remove(borrow);
                    }

                    foreach (var purchase in purchases)
                    {
                        db.Purchases.Remove(purchase);
                    }

                    // מחיקת המשתמש עצמו
                    db.Users.Remove(user);

                    // שמירת השינויים
                    db.SaveChanges();
                }
                TempData["Message"] = "User deleted successfully.";
                return RedirectToAction("ViewUsers");
            }
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            using (var db = new LibraryContext())
            {
                var user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                var model = new EditUserViewModel
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                return View(model);
            }
        }



        [HttpPost]
        public ActionResult EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new LibraryContext())
            {
                var user = db.Users.Find(model.UserId);

                if (user == null)
                {
                    return HttpNotFound();
                }

                if (!string.IsNullOrEmpty(model.FirstName))
                {
                    user.FirstName = model.FirstName;
                }
                if (!string.IsNullOrEmpty(model.LastName))
                {
                    user.LastName = model.LastName;
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                }

                db.SaveChanges();
            }

            TempData["Message"] = "User details updated successfully.";
            return RedirectToAction("ViewUsers");
        }





    }
}