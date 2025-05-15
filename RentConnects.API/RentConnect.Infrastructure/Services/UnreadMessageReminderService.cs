using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Services;

public class UnreadMessageReminderService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<UnreadMessageReminderService> _logger;

    public UnreadMessageReminderService(IServiceProvider services, ILogger<UnreadMessageReminderService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var fifteenMinutesAgo = DateTime.UtcNow.AddMinutes(-15);

            var messages = await db.Messages
                .Where(m => !m.IsRead && !m.EmailReminderSent && m.Timestamp <= fifteenMinutesAgo)
                .ToListAsync();

            foreach (var msg in messages)
            {
                try
                {
                    var receiver = await userManager.FindByIdAsync(msg.ReceiverId);
                    if (receiver != null)
                    {
                        var emailBody = $"""
                            Hello {receiver.FullName},<br/><br/>
                            You have a new unread message that was sent to you.<br/>
                            Please check it here:<br/>
                            <a href="https://localhost:7166/messages">Go to Messages</a><br/><br/>
                            RentConnect Team
                        """;

                        await emailService.SendEmailAsync(receiver.Email, "You have an unread message", emailBody);
                        msg.EmailReminderSent = true;
                        _logger.LogInformation($"Email sent to {receiver.Email} for message {msg.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send reminder email: {ex.Message}");
                }
            }

            await db.SaveChangesAsync();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // ينتظر 5 دقايق ويرجع يشيّك
        }
    }
}
