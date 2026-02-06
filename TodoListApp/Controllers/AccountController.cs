using Microsoft.AspNetCore.Mvc;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService service;
    private readonly IEmailService emailService;

    public AccountController(IAccountService service, IEmailService emailService)
    {
        this.service = service;
        this.emailService = emailService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var result = await this.service.RegisterAsync(model);
        if (result.Succeeded)
        {
            return this.Ok(new { Message = "Користувач успішно зареєстрований!" });
        }

        return this.BadRequest(result.Errors);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var token = await this.service.LoginAsync(model);
        if (token == null)
        {
            return this.Unauthorized(new { Message = "Некоректний Email або пароль." });
        }

        return this.Ok(new { token = token });
    }

    [HttpPost]
    [Route("verifyEmail")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailViewModel model)
    {
        var token = await this.service.VerifyEmailAndGenerateToken(model);

        if (token == null)
        {
            return this.NotFound("Користувача не знайдено");
        }

        var resetLink = $"https://localhost:7253/AccountApp/ChangePassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(model.Email)}";

        var body = $"Для скидання пароля перейдіть за посиланням: <a href='{resetLink}'>Скинути</a>";

        await this.emailService.SendEmailAsync(model.Email, "Відновлення пароля", body);

        return this.Ok();
    }

    [HttpPost]
    [Route("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var result = await this.service.ResetPasswordAsync(model);

        if (result.Succeeded)
        {
            return this.Ok("Пароль успішно змінено");
        }

        return this.BadRequest(result.Errors);
    }
}
