using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Controllers;

public class TodoListController : Controller
{
    private readonly ITodoListWebApiService service;

    public TodoListController(ITodoListWebApiService service)
    {
        this.service = service;
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
    public async Task<IActionResult> Create(TodoListModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            await this.service.CreateAsync(model);
            return this.RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            this.ModelState.AddModelError(string.Empty, $"Не вдалося створити список: {ex.Message}");
            return this.View(model);
        }
    }

    // GET: TodoList/Edit/5
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

    // POST: TodoList/Edit/5
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
            this.ModelState.AddModelError(string.Empty, $"Не вдалося оновити список: {ex.Message}");
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
            return this.RedirectToAction(nameof(this.Index));
        }
        catch (HttpRequestException ex)
        {
            this.TempData["Error"] = $"Не вдалося видалити: {ex.Message}";
            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
