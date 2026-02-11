using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApp.Helpers;

namespace TodoListApp.WebApp.Controllers;

[JwtAuthorize]
public class TodoListController : BaseController
{
    private readonly ITodoListWebApiService service;
    private readonly IUserWebApiService userService;

    public TodoListController(ITodoListWebApiService service, IUserWebApiService userService)
    {
        this.service = service;
        this.userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var todoLists = await this.service.GetAllAsync();
        return this.View(todoLists);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoListModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            await this.service.CreateAsync(model);
            return this.RedirectToAction(nameof(this.Index));
        }
        catch (ArgumentException ex)
        {
            this.HandleException(ex, string.Empty);
            return this.View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await this.service.GetByIdAsync(id);

        if (model == null)
        {
            return this.NotFound();
        }

        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TodoListModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            await this.service.UpdateAsync(model);
            return this.RedirectToAction(nameof(this.Index));
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex, string.Empty);
            return this.View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await this.service.DeleteByIdAsync(id);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddMember(int todoListId, string userId)
    {
        try
        {
            await this.service.AddMemberAsync(todoListId, userId);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction("Index", "TasksApp", new { todolistId = todoListId });
    }

    [HttpPost("RemoveMember")]
    public async Task<IActionResult> RemoveMember(int todoListId, string memberId)
    {
        try
        {
            await this.service.RemoveMemberAsync(todoListId, memberId);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction("Index", "TasksApp", new { todolistId = todoListId });
    }

    [HttpPost("ChangeRole")]
    public async Task<IActionResult> UpdateRole(int todoListId, string memberId, TodoListRole newRole)
    {
        try
        {
            await this.service.UpdateMemberRoleAsync(todoListId, memberId, newRole);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction("Index", "TasksApp", new { todolistId = todoListId });
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersJson(string query)
    {
        try
        {
            var users = await this.userService.SearchUsersAsync(query);
            return this.Json(users);
        }
        catch (HttpRequestException)
        {
            return this.Json(new List<UserLookupModel>());
        }
    }
}
