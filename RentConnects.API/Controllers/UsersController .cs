using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using System.Text;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using static RentConnect.API.RentConnect.Presentation.DTOs.ReviewDetailsDto;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IEmailService _emailService;

    private readonly IConfiguration _config;
    public UsersController(UserManager<ApplicationUser> userManager, IEmailService emailService, IConfiguration config)
    {
        _userManager = userManager;
        _emailService = emailService;
        _config = config;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.Users.Select(u => new
        {
            u.Id,
            u.UserName,
            u.Email,
            u.FullName
        }).ToList();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.FullName
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user)
    {
        var result = await _userManager.CreateAsync(user, "DefaultPassword123!");
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser updatedUser)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.FullName = updatedUser.FullName;
        user.Email = updatedUser.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            return Ok(); // Do not reveal if the user doesn't exist or email is not confirmed
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var callbackUrl = $"https://localhost:7166/Account/ResetPassword?code={encodedToken}";

        // Send the email
        await _emailService.SendEmailAsync(
            to: user.Email,
            subject: "Reset Your Password",
            body: $"Click the link to reset your password: <a href='{callbackUrl}'>Reset Password</a>"
        );

        return Ok(new { Message = "Reset link has been sent to your email." });
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Do not reveal that the user does not exist
            return Ok(new { Message = "Reset successful." });
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        Console.WriteLine($"Encoded token: {model.Code}");
        Console.WriteLine($"Decoded token: {decodedToken}");


        return Ok(new { Message = "Password has been reset successfully." });
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Code { get; set; } = "";
    }

    public class ConfirmEmailRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        await _emailService.SendEmailAsync(
            "your-own-email@gmail.com",
            "Test Email from API",
            "<h1>This is a test email</h1><p>It was sent from your backend.</p>"
        );

        return Ok("Test email sent");
    }


    [HttpPost("send-confirmation-email")]
    public async Task<IActionResult> SendConfirmationEmail([FromBody] ConfirmEmailRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (await _userManager.IsEmailConfirmedAsync(user))
        {
            return Ok(new { Message = "Email is already confirmed." });
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = $"https://localhost:7166/Account/ConfirmEmail?userId={user.Id}&code={encodedToken}";

        await _emailService.SendEmailAsync(
            to: user.Email,
            subject: "Confirm Your Email",
            body: $"Click to confirm your email: <a href='{callbackUrl}'>Confirm Email</a>"
        );

        Console.WriteLine("✅ Confirmation link: " + callbackUrl); // Debug link if email fails

        return Ok(new { Message = "Confirmation email sent. Please check your inbox." });
    }



}

