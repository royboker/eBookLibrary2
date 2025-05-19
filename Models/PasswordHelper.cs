using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace eBookLibrary3.Models
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }

            using (var sha256 = SHA256.Create())
            {
                // המרה של הסיסמה למערך בתים
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                // יצירת ה-Hash
                byte[] hash = sha256.ComputeHash(bytes);

                // המרה למחרוזת Hexadecimal
                StringBuilder builder = new StringBuilder();
                foreach (var b in hash)
                {
                    builder.Append(b.ToString("x2")); // המרה לפורמט Hexadecimal
                }

                return builder.ToString();
            }
        }
    }
}