using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Configuration;

namespace durashopcommunication.Utils
{
    public class Mail
    {
        internal static async void Send(string to, string from, string subject, string body)
        {
            var apiKey = ConfigurationManager.AppSettings["sendgrid-key"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(from, "DuraShop Team"),
                Subject = subject,
                PlainTextContent = body
            };
            msg.AddTo(new EmailAddress(to, "Arne Anka"));
            var response = await client.SendEmailAsync(msg);
        }
    }
}
