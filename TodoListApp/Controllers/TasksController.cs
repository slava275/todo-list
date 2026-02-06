using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Exceptions;
using TodoListApp.Extensions;
using TodoListApp.Interfaces;
using TodoListShared.Models;
using TodoListShared.Models.Models;

namespace TodoListApp.Controllers;

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
        try
        {
            var task = await this.service.GetByIdAsync(id, this.UserId);
            return this.Ok(task);
        }
        catch (EntityNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (AccessDeniedException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TaskModel task)
    {
        if (task == null)
        {
            return this.BadRequest();
        }

        try
        {
            await this.service.CreateAsync(task, this.UserId);
            return this.CreatedAtAction(nameof(this.GetById), new { id = task.Id }, task);
        }
        catch (EntityNotFoundException ex)
        {
            // Наприклад, якщо вказано неіснуючий TodoListId
            return this.NotFound(ex.Message);
        }
        catch (AccessDeniedException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] TaskModel task)
    {
        if (task == null || task.Id != id)
        {
            return this.BadRequest();
        }

        try
        {
            await this.service.UpdateAsync(task, this.UserId);
            return this.NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (AccessDeniedException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await this.service.DeleteByIdAsync(id, this.UserId);
            return this.NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (AccessDeniedException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpGet("list/{todoListId}")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetByTodoListId(int todoListId)
    {
        // Сервіс повертає порожній список, якщо доступу немає
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
        try
        {
            await this.service.ChangeStatusAsync(id, newStatus, this.UserId);
            return this.NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (AccessDeniedException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
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
}
