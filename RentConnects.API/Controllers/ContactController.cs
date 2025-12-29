using Microsoft.AspNetCore.Mvc;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using RentConnect.API.RentConnect.Presentation.DTOs;

namespace RentConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public ContactController(IEmailService emailService, IConfiguration config)
        {
            _emailService = emailService;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ContactRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // الإيميل اللي تستقبل عليه رسائل التواصل
            // تقدر تغيّره لاحقاً لإيميل ثاني لو حبيت
            var supportEmail = _config["Smtp:Username"] ?? "rentconnectab@gmail.com";

            var subject = $"[Contact] {dto.Subject}";

            var body = $"""
                <p>New contact message from <strong>{dto.FullName}</strong> ({dto.Email})</p>
                <p><strong>Topic:</strong> {dto.Topic}</p>
                <hr />
                <p style="white-space: pre-line;">{dto.Message}</p>
                <hr />
                <p>Sent from RentConnect contact page.</p>
            """;

            await _emailService.SendEmailAsync(
                to: supportEmail,
                subject: subject,
                body: body
            );

            // ✅ (اختياري) ترسل إيميل تأكيد للمرسل نفسه
            var userBody = $"""
                Hello {dto.FullName},<br/><br/>
                We have received your message:<br/>
                <strong>Subject:</strong> {dto.Subject}<br/>
                <strong>Topic:</strong> {dto.Topic}<br/><br/>
                Our team will review it and get back to you as soon as possible.<br/><br/>
                Thanks,<br/>
                RentConnect Support
            """;

            await _emailService.SendEmailAsync(
                to: dto.Email,
                subject: "We received your message",
                body: userBody
            );

            return Ok(new { message = "Contact message sent successfully." });
        }
    }
}
