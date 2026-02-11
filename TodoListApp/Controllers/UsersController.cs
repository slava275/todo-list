using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserLookupModel>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentNullException(query);
        }

        if (query.Length < 3)
        {
            throw new ArgumentException("Для пошуку потрібно мінімум 3 символи.");
        }

        var results = await this.userService.SearchUsersAsync(query);
        return this.Ok(results);
    }
}
