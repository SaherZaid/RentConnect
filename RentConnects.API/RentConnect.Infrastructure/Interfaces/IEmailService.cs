namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);

}