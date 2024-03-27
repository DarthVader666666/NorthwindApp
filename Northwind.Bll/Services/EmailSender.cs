﻿using Microsoft.Extensions.Configuration;
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
            var mail = _configuration["Email"];
            var password = _configuration["Password"];

            var client = new SmtpClient("outlook.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            var mailMessage = new MailMessage(from: mail!, to: email, subject, message)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}