using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Controllers;

[JwtAuthorize]
[Route("TasksApp")]
public class TasksAppController : Controller
{
    private readonly ITaskWebApiService service;
    private readonly ITagWebApiService tagService;
    private readonly ICommentWebApiService commentService;

    public TasksAppController(ITaskWebApiService service, ITagWebApiService tagService, ICommentWebApiService commentService)
    {
        this.service = service;
        this.tagService = tagService;
        this.commentService = commentService;
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
            this.TempData["ErrorMessage"] = $"Не вдалося створити завдання: {ex.Message}";
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
        catch (HttpRequestException ex)
        {
            this.TempData["ErrorMessage"] = $"Не вдалося видалити: {ex.Message}";
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
            var newModel = await this.service.GetByIdAsync(model.Id);
            this.TempData["ErrorMessage"] = $"Не вдалося відредагувати завдання: {ex.Message}";
            return this.View(newModel);
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

    [HttpPost("AddTag")]
    public async Task<IActionResult> AddTag(int taskId, string tagName)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(tagName))
            {
                await this.tagService.AddTagToTaskAsync(taskId, new TagModel { Name = tagName.Trim() });
            }

            return this.RedirectToAction("Edit", new { id = taskId });
        }
        catch (HttpRequestException ex)
        {
            this.TempData["ErrorMessage"] = $"Не вдалося додати тег: {ex.Message}";
            return this.RedirectToAction("Edit", new { id = taskId });
        }
    }

    [HttpPost("RemoveTag")]
    public async Task<IActionResult> RemoveTag(int taskId, int tagId)
    {
        try
        {
            await this.tagService.RemoveTagFromTaskAsync(taskId, tagId);

            return this.RedirectToAction("Edit", new { id = taskId });
        }
        catch (HttpRequestException ex)
        {
            this.TempData["ErrorMessage"] = $"Не вдалося видалити тег: {ex.Message}";
            return this.RedirectToAction("Edit", new { id = taskId });
        }
    }

    [HttpPost("AddComment")]
    public async Task<IActionResult> AddComment(int taskId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText))
        {
            return this.RedirectToAction("Details", new { id = taskId });
        }

        var newComment = new CommentModel
        {
            TaskId = taskId,
            Text = commentText,
            CreatedAt = DateTime.Now,
        };

        try
        {
            await this.commentService.AddAsync(newComment);
            return this.RedirectToAction("Details", new { id = taskId });
        }
        catch (HttpRequestException ex)
        {
            this.TempData["ErrorMessage"] = $"Не вдалося додати коментар: {ex.Message}";
            return this.RedirectToAction("Details", new { id = taskId });
        }
    }

    [HttpPost("DeleteComment")]
    public async Task<IActionResult> DeleteComment(int commentId, int taskId)
    {
        try
        {
            await this.commentService.DeleteAsync(commentId);
            return this.RedirectToAction("Details", new { id = taskId });
        }
        catch (HttpRequestException ex)
        {
            this.TempData["ErrorMessage"] = $"Не вдалося видалити коментар: {ex.Message}";
            return this.RedirectToAction("Details", new { id = taskId });
        }
    }

    [HttpPost("EditComment")]
    public async Task<IActionResult> EditComment(int commentId, int taskId, string newText)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(newText))
            {
                await this.commentService.UpdateAsync(new CommentModel
                {
                    Id = commentId,
                    Text = newText,
                    TaskId = taskId,
                });
            }

            return this.RedirectToAction(nameof(this.Details), new { id = taskId });
        }
        catch (HttpRequestException ex)
        {
            this.TempData["ErrorMessage"] = $"Не вдалося оновити коментар: {ex.Message}";
            return this.RedirectToAction("Details", new { id = taskId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Assign(int taskId, string userId, int todolistId)
    {
        try
        {
            await this.service.AssignTaskAsync(taskId, userId);
        }
        catch (HttpRequestException ex)
        {
            this.TempData["ErrorMessage"] = "Не вдалося призначити виконавця: " + ex.Message;
        }

        return this.RedirectToAction("Index", new { todolistId = todolistId });
    }
}
