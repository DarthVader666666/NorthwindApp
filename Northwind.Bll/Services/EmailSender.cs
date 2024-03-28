using Microsoft.Extensions.Configuration;
using Northwind.Bll.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Northwind.Bll.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            //var mail = _configuration["Email"];
            //var password = _configuration["Password"];

            //var client = new SmtpClient("outlook.office365.com", 587)
            //{
            //    EnableSsl = true,
            //    Credentials = new NetworkCredential(mail, password)
            //};

            //var mailMessage = new MailMessage(from: mail!, to: email, subject, message)
            //{
            //    IsBodyHtml = true
            //};

            //string smtpAuthUsername = "azure-vader-communication-service";
            //string smtpAuthPassword = "I7O8Q~q1c2We3k-kOccmKaoeZpSr-Fk.5oJX3dg0";
            //string sender = "DoNotReply@00a2ce84-68f9-47ff-a5f8-cc87e989435c.azurecomm.net";
            //string smtpHostUrl = "smtp.azurecomm.net";

            //var client = new SmtpClient(smtpHostUrl)
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential(smtpAuthUsername, smtpAuthPassword),
            //    EnableSsl = true
            //};

            //await client.SendMailAsync(new MailMessage(sender, email, subject, message));
        }
    }
}
