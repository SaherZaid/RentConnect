using Microsoft.AspNetCore.Identity;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class EmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailService _emailService;

    public EmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string link)
    {
        var body = $"""
                        Hello {user.FullName},<br/><br/>
                        Please confirm your email by clicking the link below:<br/>
                        <a href='{link}'>Confirm Email</a><br/><br/>
                        If you did not request this, please ignore this email.<br/><br/>
                        Thanks!
                    """;

        await _emailService.SendEmailAsync(
            to: email,
            subject: "Confirm your email address",
            body: body);
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }

    public Task SendEmailAsync(ApplicationUser user, string email, string subject, string htmlMessage)
    {
        return _emailService.SendEmailAsync(email, subject, htmlMessage);
    }

    // Optional: implement if you're sending password reset emails from Identity UI (not required for now)
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetLink)
    {
        return _emailService.SendEmailAsync(email, "Reset Password", resetLink);
    }
}