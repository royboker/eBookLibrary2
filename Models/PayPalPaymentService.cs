using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary3.Models
{
    public class PayPalPaymentService
    {
        private static APIContext GetApiContext()
        {
            var config = new Dictionary<string, string>
        {
            { "mode", System.Configuration.ConfigurationManager.AppSettings["PayPalMode"] },
            { "clientId", System.Configuration.ConfigurationManager.AppSettings["PayPalClientId"] },
            { "clientSecret", System.Configuration.ConfigurationManager.AppSettings["PayPalSecret"] }
        };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            return new APIContext(accessToken);
        }

        public Payment CreatePayment(string returnUrl, string cancelUrl, decimal amount)
        {
            var apiContext = GetApiContext();

            var payer = new Payer() { payment_method = "paypal" };

            var redirectUrls = new RedirectUrls()
            {
                cancel_url = cancelUrl,
                return_url = returnUrl
            };

            var amountDetails = new Amount()
            {
                currency = "USD",
                total = amount.ToString("F2")
            };

            var transaction = new Transaction()
            {
                description = "Purchase from My Website",
                amount = amountDetails
            };

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = new List<Transaction> { transaction },
                redirect_urls = redirectUrls
            };

            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetApiContext();
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);

        }
    }
}