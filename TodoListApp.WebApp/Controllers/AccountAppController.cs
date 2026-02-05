using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Controllers;

[AllowAnonymous]
[Route("AccountApp")]
public class AccountAppController : Controller
{
    private readonly IAuthService authService;

    public AccountAppController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        if (this.Request.Cookies.ContainsKey("jwtToken"))
        {
            return this.RedirectToAction("Index", "TodoList");
        }

        return this.View();
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login(LoginViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var result = await this.authService.LoginAsync(model);

        if (result)
        {
            return this.RedirectToAction("Index", "TodoList");
        }

        this.ModelState.AddModelError(string.Empty, "Невірний логін або пароль");
        return this.View(model);
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        return this.View();
    }

    [HttpPost("Register")]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var result = await this.authService.RegisterAsync(model);
        if (result)
        {
            return this.RedirectToAction("Login");
        }

        this.ModelState.AddModelError(string.Empty, "Помилка при реєстрації користувача");
        return this.View(model);
    }

    [JwtAuthorize]
    [HttpPost("Logout")]
    public async Task<ActionResult> Logout()
    {
        await this.authService.LogoutAsync();
        return this.RedirectToAction("Login");
    }
}
