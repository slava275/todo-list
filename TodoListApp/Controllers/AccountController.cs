using Microsoft.AspNetCore.Mvc;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService service;

    public AccountController(IAccountService service)
    {
        this.service = service;
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
}
