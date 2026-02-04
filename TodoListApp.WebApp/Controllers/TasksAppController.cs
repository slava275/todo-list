using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Controllers;

[Route("TasksApp")]
public class TasksAppController : Controller
{
    private readonly ITaskWebApiService service;

    public TasksAppController(ITaskWebApiService service)
    {
        this.service = service;
    }

    [HttpGet("List/{todolistId}")]
    public async Task<IActionResult> Index(int todolistId)
    {
        try
        {
            var models = await this.service.GetAllByListIdAsync(todolistId);
            this.ViewBag.TodoListId = todolistId;
            return this.View(models);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            this.ModelState.AddModelError(string.Empty, $"Не вдалося знайти завдання: {ex.Message}");
            return this.RedirectToAction("Index", "TodoList");
        }
    }

    [HttpGet("Create/{todolistId}")]
    public IActionResult Create(int todolistId)
    {
        var model = new TaskModel { TodoListId = todolistId };
        return this.View(model);
    }

    [HttpPost("Create/{todolistId}")]
    public async Task<IActionResult> Create(TaskModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            await this.service.CreateAsync(model);

            return this.RedirectToAction("Index", new { todolistId = model.TodoListId });
        }
        catch (HttpRequestException ex)
        {
            this.ModelState.AddModelError(string.Empty, $"Помилка: {ex.Message}");
            return this.View(model);
        }
    }

    [HttpPost("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int todolistId)
    {
        try
        {
            await this.service.DeleteByIdAsync(id);
            return this.RedirectToAction("Index", new { todolistId = todolistId });
        }
        catch (HttpRequestException)
        {
            return this.RedirectToAction("Index", new { todolistId = todolistId });
        }
    }

    // GET: TasksApp/Edit/5
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var task = await this.service.GetByIdAsync(id);
        if (task == null)
        {
            return this.NotFound();
        }

        return this.View(task);
    }

    // POST: TasksApp/Edit/5
    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, TaskModel model)
    {
        if (id != model.Id)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            await this.service.UpdateAsync(model);
            return this.RedirectToAction("Index", new { todolistId = model.TodoListId });
        }
        catch (HttpRequestException ex)
        {
            this.ModelState.AddModelError(string.Empty, $"Помилка оновлення: {ex.Message}");
            return this.View(model);
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var task = await this.service.GetByIdAsync(id);
        if (task == null)
        {
            return this.NotFound();
        }

        return this.View(task);
    }

    [HttpGet("Assigned")]
    public async Task<IActionResult> Assigned(Statuses? statusFilter, string sortBy = "name", bool isAscending = true)
    {
        Statuses? filter = statusFilter;

        var tasks = await this.service.GetAllByUserIdAsync(filter, sortBy, isAscending);

        this.ViewBag.CurrentFilter = filter;
        this.ViewBag.CurrentSort = sortBy;
        this.ViewBag.CurrentIsAscending = isAscending;

        return this.View(tasks);
    }

    [HttpGet("Search")]
    public async Task<IActionResult> Search(string? title, DateTime? dueDate, DateTime? createdAt)
    {
        var results = await this.service.SearchAsync(title, dueDate, createdAt);

        this.ViewBag.TitleFilter = title;
        this.ViewBag.DueDateFilter = dueDate;
        this.ViewBag.CreatedAtFilter = createdAt;

        return this.View(results);
    }
}
