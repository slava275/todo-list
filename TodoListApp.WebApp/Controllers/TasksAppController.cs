using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Controllers;

[JwtAuthorize]
[Route("TasksApp")]
public class TasksAppController : BaseController
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
            this.HandleException(ex);
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            await this.service.CreateAsync(model);
            return this.RedirectToAction(nameof(this.Index), new { todolistId = model.TodoListId });
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex, string.Empty);
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
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Index), new { todolistId = todolistId });
    }

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

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
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
            return this.RedirectToAction(nameof(this.Index), new { todolistId = model.TodoListId });
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex, string.Empty);

            var taskFromDb = await this.service.GetByIdAsync(model.Id);
            return this.View(taskFromDb);
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
        var tasks = await this.service.GetAllByUserIdAsync(statusFilter, sortBy, isAscending);

        this.ViewBag.CurrentFilter = statusFilter;
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTag(int taskId, string tagName)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(tagName))
            {
                await this.tagService.AddTagToTaskAsync(taskId, new TagModel { Name = tagName.Trim() });
            }
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Edit), new { id = taskId });
    }

    [HttpPost("RemoveTag")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTag(int taskId, int tagId)
    {
        try
        {
            await this.tagService.RemoveTagFromTaskAsync(taskId, tagId);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Edit), new { id = taskId });
    }

    [HttpPost("AddComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int taskId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText))
        {
            return this.RedirectToAction(nameof(this.Details), new { id = taskId });
        }

        try
        {
            var newComment = new CommentModel
            {
                TaskId = taskId,
                Text = commentText,
                CreatedAt = DateTime.UtcNow,
            };

            await this.commentService.AddAsync(newComment);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Details), new { id = taskId });
    }

    [HttpPost("DeleteComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int commentId, int taskId)
    {
        try
        {
            await this.commentService.DeleteAsync(commentId);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Details), new { id = taskId });
    }

    [HttpPost("EditComment")]
    [ValidateAntiForgeryToken]
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
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Details), new { id = taskId });
    }

    [HttpPost("Assign")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int taskId, string userId, int todolistId)
    {
        try
        {
            await this.service.AssignTaskAsync(taskId, userId);
        }
        catch (HttpRequestException ex)
        {
            this.HandleException(ex);
        }

        return this.RedirectToAction(nameof(this.Index), new { todolistId = todolistId });
    }
}
