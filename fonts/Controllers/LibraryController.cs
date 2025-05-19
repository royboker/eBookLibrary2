//using eBookLibrary3.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace eBookLibrary3.Controllers
//{
//    public class LibraryController : Controller
//    {

//        public ActionResult ShowLibrary(string searchTerm, string sortOrder, string genre)
//        {
//            // יצירת הקשר עם מסד הנתונים בתוך הפעולה
//            using (var db = new LibraryContext())
//            {
//                // שאילתה בסיסית של הספרים
//                var books = db.Books.AsQueryable();

//                // חיפוש חופשי (כולל שם הספר, שם המחבר, או שם המוציא לאור)
//                if (!string.IsNullOrEmpty(searchTerm))
//                {
//                    books = books.Where(b => b.Title.Contains(searchTerm) ||
//                                             b.Author.Contains(searchTerm) ||
//                                             b.Publisher.Contains(searchTerm));
//                }

//                // סינון לפי ז'אנר
//                if (!string.IsNullOrEmpty(genre))
//                {
//                    books = books.Where(b => b.Genre == genre);
//                }

//                // מיון
//                switch (sortOrder)
//                {
//                    case "price_asc":
//                        books = books.OrderBy(b => b.BuyPrice);
//                        break;
//                    case "price_desc":
//                        books = books.OrderByDescending(b => b.BuyPrice);
//                        break;
//                    case "popularity":
//                        books = books.OrderByDescending(b => b.Popularity);
//                        break;
//                    case "year":
//                        books = books.OrderByDescending(b => b.YearOfPublication);
//                        break;
//                }

//                // שליפת רשימת ז'אנרים ל-ViewBag
//                ViewBag.Genres = db.Books.Select(b => b.Genre).Distinct().ToList();

//                // החזרת הספרים ל-View
//                return View(books.ToList());
//            }
//        }



//        public ActionResult BookDetails(int id)
//        {
//            using (var db = new LibraryContext())
//            {
//                var book = db.Books.FirstOrDefault(b => b.BookId == id);

//                if (book == null)
//                {
//                    return HttpNotFound();
//                }

//                return View(book);
//            }
//        }

//        // פעולה לטיפול בחיפוש
//        public ActionResult SearchBooks(string searchQuery)
//        {
//            // יצירת הקשר עם מסד הנתונים בתוך הפעולה
//            using (var db = new LibraryContext())
//            {
//                // בדיקת שאילתת החיפוש
//                if (string.IsNullOrEmpty(searchQuery))
//                {
//                    // אם אין חיפוש, חזרה לכל הספרים
//                    return RedirectToAction("ShowLibrary");
//                }

//                // חיפוש ספרים לפי כותרת, מחבר, או ז'אנר
//                var books = db.Books
//                    .Where(b => b.Title.Contains(searchQuery) ||
//                                b.Author.Contains(searchQuery) ||
//                                b.Genre.Contains(searchQuery))
//                    .ToList();

//                // החזרת תצוגה עם הספרים שנמצאו
//                return View("ShowLibrary", books);
//            }
//        }
//    }
//}

using eBookLibrary3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBookLibrary3.Controllers
{
    public class LibraryController : Controller
    {
        // GET: HomePage
        public ActionResult ShowLibrary(string searchTerm, string sortOrder, string genre, string purchaseType, decimal? minPrice, decimal? maxPrice, bool? discounted)
        {
            // יצירת הקשר עם מסד הנתונים בתוך הפעולה
            using (var db = new LibraryContext())
            {
                // שאילתה בסיסית של הספרים
                var books = db.Books.AsQueryable();

                // חיפוש חופשי (כולל שם הספר, שם המחבר, או שם המוציא לאור)
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    books = books.Where(b => b.Title.Contains(searchTerm) ||
                                             b.Author.Contains(searchTerm) ||
                                             b.Publisher.Contains(searchTerm));
                }

                // סינון לפי ז'אנר
                if (!string.IsNullOrEmpty(genre))
                {
                    books = books.Where(b => b.Genre == genre);
                }

                // סינון לפי סוג קנייה (רק לקנייה או רק להשאלה)
                if (!string.IsNullOrEmpty(purchaseType))
                {
                    if (purchaseType == "Buy")
                    {
                        books = books.Where(b => b.BorrowPrice ==0);
                    }
                    else if (purchaseType == "Borrow")
                    {
                        books = books.Where(b => b.BorrowPrice!=0);
                    }
                }

                // סינון לפי טווח מחירים
                if (minPrice.HasValue)
                {
                    books = books.Where(b => b.BuyPrice >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    books = books.Where(b => b.BuyPrice <= maxPrice.Value);
                }

                // סינון לפי מחיר מוזל
                if (discounted.HasValue && discounted.Value)
                {
                    books = books.Where(b => b.BuyPrice < b.Price);
                }

                // מיון
                switch (sortOrder)
                {
                    case "price_asc":
                        books = books.OrderBy(b => b.BuyPrice);
                        break;
                    case "price_desc":
                        books = books.OrderByDescending(b => b.BuyPrice);
                        break;
                    case "popularity":
                        books = books.OrderByDescending(b => b.Popularity);
                        break;
                    case "year":
                        books = books.OrderByDescending(b => b.YearOfPublication);
                        break;
                }

                // שליפת רשימת ז'אנרים ל-ViewBag
                ViewBag.Genres = db.Books.Select(b => b.Genre).Distinct().ToList();

                // החזרת הספרים ל-View
                return View(books.ToList());
            }
        }

        //public ActionResult BookDetails(int id)
        //{
        //    using (var db = new LibraryContext())
        //    {
        //        var book = db.Books.FirstOrDefault(b => b.BookId == id);

        //        if (book == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        // חישוב מספר הפעמים שהספר נקנה
        //        var purchaseCount = db.Purchases.Count(p => p.BookId == id);
        //        ViewBag.PurchaseCount = purchaseCount;

        //        if (book.DiscountStartDate != null)
        //        {
        //            DateTime discountStartDate = book.DiscountStartDate.Value;
        //            DateTime discountEndDate = discountStartDate.AddDays(7); // ההנחה תקפה לשבוע

        //            if (DateTime.Now >= discountStartDate && DateTime.Now <= discountEndDate) // האם ההנחה בתוקף
        //            {
        //                int daysLeft = (int)(discountEndDate - DateTime.Now).TotalDays; // חישוב הימים שנותרו
        //                TempData["SaleMessage"] = $"Hurry Up! Only {daysLeft + 1} days left for the sale!"; // +1 כי הספירה מתחילה מ-0
        //            }
        //            else if (DateTime.Now < discountStartDate) // ההנחה עדיין לא התחילה
        //            {
        //                int daysUntilStart = (int)(discountStartDate - DateTime.Now).TotalDays;
        //                TempData["SaleMessage"] = $"Hurry Up! Only {daysUntilStart + 1} days to the sale left!";
        //            }
        //        }

        //        // שליפת רשימת ההמתנה עבור הספר
        //        var waitingList = db.WaitingLists
        //            .Where(w => w.BookId == id)
        //            .OrderBy(w => w.Position)
        //            .Select(w => new WaitingListViewModel
        //            {
        //                UserName = w.User.FirstName + " " + w.User.LastName,
        //                Position = w.Position
        //            })
        //            .ToList();

        //        ViewBag.WaitingList = waitingList; // העברת רשימת ההמתנ



        //        return View(book);
        //    }
        //}

        public ActionResult BookDetails(int id)
        {
            using (var db = new LibraryContext())
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == id);

                if (book == null)
                {
                    return HttpNotFound();
                }

                // חישוב מספר הפעמים שהספר נקנה
                var purchaseCount = db.Purchases.Count(p => p.BookId == id);
                ViewBag.PurchaseCount = purchaseCount;

                if (book.DiscountStartDate != null)
                {
                    DateTime discountStartDate = book.DiscountStartDate.Value;
                    DateTime discountEndDate = discountStartDate.AddDays(7); // ההנחה תקפה לשבוע

                    if (DateTime.Now >= discountStartDate && DateTime.Now <= discountEndDate) // האם ההנחה בתוקף
                    {
                        int daysLeft = (int)(discountEndDate - DateTime.Now).TotalDays; // חישוב הימים שנותרו
                        TempData["SaleMessage"] = $"Hurry Up! Only {daysLeft + 1} days left for the sale!"; // +1 כי הספירה מתחילה מ-0
                    }
                    else if (DateTime.Now < discountStartDate) // ההנחה עדיין לא התחילה
                    {
                        int daysUntilStart = (int)(discountStartDate - DateTime.Now).TotalDays;
                        TempData["SaleMessage"] = $"Hurry Up! Only {daysUntilStart + 1} days to the sale left!";
                    }
                }

                // שליפת רשימת ההמתנה עבור הספר
                var waitingList = db.WaitingLists
                    .Where(w => w.BookId == id)
                    .OrderBy(w => w.Position)
                    .ToList(); // שליפת הנתונים כ-List

                // חישוב EstimatedAvailability ב-C#
                var waitingListViewModel = waitingList
                    .Select((w, index) => new WaitingListViewModel
                    {
                        UserName = w.User.FirstName + " " + w.User.LastName,
                        EstimatedAvailability = DateTime.Now.AddDays((index + 1) * 30) // כל משתמש יקבל תאריך על פי מיקומו ברשימה
                    })
                    .ToList();

                ViewBag.WaitingList = waitingListViewModel; // העברת רשימת ההמתנה ל-ViewBag

                return View(book);
            }
        }



        // פעולה לטיפול בחיפוש
        public ActionResult SearchBooks(string searchQuery)
        {
            // יצירת הקשר עם מסד הנתונים בתוך הפעולה
            using (var db = new LibraryContext())
            {
                // בדיקת שאילתת החיפוש
                if (string.IsNullOrEmpty(searchQuery))
                {
                    // אם אין חיפוש, חזרה לכל הספרים
                    return RedirectToAction("ShowLibrary");
                }

                // חיפוש ספרים לפי כותרת, מחבר, או ז'אנר
                var books = db.Books
                    .Where(b => b.Title.Contains(searchQuery) ||
                                b.Author.Contains(searchQuery) ||
                                b.Genre.Contains(searchQuery))
                    .ToList();

                // החזרת תצוגה עם הספרים שנמצאו
                return View("ShowLibrary", books);
            }
        }
    }
}
