using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace My_ticket.Request
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com") // أو smtp الخاص بمزودك
            {
                Port = 587,
                Credentials = new NetworkCredential("amirtharwat321@gmail.com", "flalqbfdgsgvjjfh"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("amirtharwat321@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

}
