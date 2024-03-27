namespace Northwind.Bll.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);
    }
}