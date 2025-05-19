using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public static class CartHelper
    {
        public static List<CartItem> GetCart()
        {
            if (HttpContext.Current.Session["Cart"] == null)
            {
                HttpContext.Current.Session["Cart"] = new List<CartItem>();
            }
            return HttpContext.Current.Session["Cart"] as List<CartItem>;
        }

        public static void AddToCart(CartItem item)
        {
            var cart = GetCart();
            cart.Add(item);
            HttpContext.Current.Session["Cart"] = cart;
        }

        public static void RemoveFromCart(int bookId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                cart.Remove(item);
            }
            HttpContext.Current.Session["Cart"] = cart;
        }

        public static void ClearCart()
        {
            HttpContext.Current.Session["Cart"] = new List<CartItem>();
        }

        public static void SaveCartToTemporary()
        {
            var cart = GetCart();
            HttpContext.Current.Session["TemporaryCart"] = new List<CartItem>(cart);
        }

        public static void RestoreCartFromTemporary()
        {
            if (HttpContext.Current.Session["TemporaryCart"] != null)
            {
                var tempCart = HttpContext.Current.Session["TemporaryCart"] as List<CartItem>;
                HttpContext.Current.Session["Cart"] = tempCart ?? new List<CartItem>();
                HttpContext.Current.Session["TemporaryCart"] = null;
            }
        }

        public static void ClearCartExcept(int bookId)
        {
            var cart = GetCart();
            var selectedItem = cart.FirstOrDefault(i => i.BookId == bookId);
            if (selectedItem != null)
            {
                HttpContext.Current.Session["Cart"] = new List<CartItem> { selectedItem };
            }
            else
            {
                ClearCart();
            }
        }


    }
}