using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskDatabaseService service;

    public TasksController(ITaskDatabaseService service)
    {
        this.service = service;
    }

    private string UserId => this.User.GetUserId();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetAll()
    {
        var tasks = await this.service.GetAllAsync(this.UserId);
        return this.Ok(tasks ?? Enumerable.Empty<TaskModel>());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskModel>> GetById(int id)
    {
        var task = await this.service.GetByIdAsync(id, this.UserId);
        return this.Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TaskModel task)
    {
        ArgumentNullException.ThrowIfNull(task);

        await this.service.CreateAsync(task, this.UserId);
        return this.CreatedAtAction(nameof(this.GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] TaskModel task)
    {
        if (task == null || task.Id != id)
        {
            throw new ArgumentException("The ID in the URL does not match the ID in the request body.");
        }

        await this.service.UpdateAsync(task, this.UserId);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await this.service.DeleteByIdAsync(id, this.UserId);
        return this.NoContent();
    }

    [HttpGet("list/{todoListId}")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetByTodoListId(int todoListId)
    {
        var tasks = await this.service.GetByListIdAsync(todoListId, this.UserId);
        return this.Ok(tasks ?? Enumerable.Empty<TaskModel>());
    }

    [HttpGet("assigned")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetAssignedTasks(
        [FromQuery] Statuses? status,
        [FromQuery] string sortBy = "name",
        [FromQuery] bool isAscending = true)
    {
        var tasks = await this.service.GetAssignedTasksAsync(this.UserId, status, sortBy, isAscending);
        return this.Ok(tasks ?? Enumerable.Empty<TaskModel>());
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateStatus(int id, [FromQuery] Statuses newStatus)
    {
        await this.service.ChangeStatusAsync(id, newStatus, this.UserId);
        return this.NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> SearchTasks(
        [FromQuery] string? title,
        [FromQuery] DateTime? dueDate,
        [FromQuery] DateTime? createdAt)
    {
        var tasks = await this.service.SerchTasksAsync(title, dueDate, createdAt, this.UserId);
        return this.Ok(tasks ?? Enumerable.Empty<TaskModel>());
    }

    [HttpPost("{id}/assign")]
    public async Task<ActionResult> AssignTask(int id, [FromQuery] string assigneeId)
    {
        if (string.IsNullOrWhiteSpace(assigneeId))
        {
            throw new ArgumentException("User ID cannot be null or empty.");
        }

        await this.service.AssignTaskAsync(id, assigneeId, this.UserId);
        return this.NoContent();
    }
}
