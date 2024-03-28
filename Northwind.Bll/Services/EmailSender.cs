using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Northwind.Bll.Interfaces;

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
            var sender = _configuration["AzureEmailSender"];
            var connectionString = _configuration["ConnectionStrings:AzureCommunicationService"];

            var client = new EmailClient(connectionString);
            
            var emailSendOperation = await client.SendAsync(Azure.WaitUntil.Completed, sender, email, subject, message);
            //EmailSendResult status = emailSendOperation.Value;
        }
    }
}
