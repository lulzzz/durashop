using Microsoft.Azure.WebJobs.Host;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace duranotification
{
    internal class Mail
    {
        internal static async void Send(string to, string from, string subject, string body, TraceWriter log)
        {
            var apiKey = Environment.GetEnvironmentVariable("sendgrid-key");

            if (string.IsNullOrEmpty(from)) { from = Environment.GetEnvironmentVariable("default-frommail"); };

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(from, "DuraShop Team"),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = $"<strong>{body}</strong>"
            };
            msg.AddTo(new EmailAddress(to, "Arne Anka"));
            var response = await client.SendEmailAsync(msg);


        }
    }
}