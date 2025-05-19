using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace eBookLibrary3.Models
{
    public class EmailService
    {
        private string smtpServer;
        private int smtpPort;
        private string senderEmail;
        private string senderPassword;

        public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderPassword)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.senderEmail = senderEmail;
            this.senderPassword = senderPassword;
        }

        public void SendEmail(string recipientEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                // יצירת הודעת המייל
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(recipientEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = isHtml;

                // הגדרת ה-SMTP
                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true;

                // שליחת המייל
                smtpClient.Send(mail);
                Console.WriteLine("המייל נשלח בהצלחה!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שליחת המייל נכשלה. שגיאה: {ex.Message}");
            }
        }
    }
}