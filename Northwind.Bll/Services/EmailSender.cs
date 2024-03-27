using Northwind.Bll.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Northwind.Bll.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            try 
            {
                var mail = "rumyancer@outlook.com";
                var password = "microsoft-Platform-A10B11";

                var client = new SmtpClient("outlook.office365.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mail, password)
                };

                var mailMessage = new MailMessage(from: mail, to: email, subject, message)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);

                return true;
            }
            catch 
            {
                return false;
            }            
        }
    }
}
