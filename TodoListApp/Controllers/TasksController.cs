using Microsoft.AspNetCore.Mvc;
using TodoListApp.Data.Models;
using TodoListApp.Interfaces;

namespace TodoListApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskDatabaseService service;

    public TasksController(ITaskDatabaseService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetAll()
    {
        var tasks = await this.service.GetAllAsync();

        if (tasks is null || !tasks.Any())
        {
            return this.NotFound("No todo lists found.");
        }

        return this.Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskModel>> GetById(int id)
    {
        var task = await this.service.GetByIdAsync(id);
        if (task is null)
        {
            return this.NotFound($"Task with ID {id} not found.");
        }

        return this.Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TaskModel task)
    {
        if (task is null)
        {
            return this.BadRequest("Task cannot be null.");
        }

        await this.service.CreateAsync(task);
        return this.CreatedAtAction(nameof(this.GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] TaskModel task)
    {
        if (task is null || task.Id != id)
        {
            return this.BadRequest("Invalid task data.");
        }

        var existingTask = await this.service.GetByIdAsync(id);
        if (existingTask is null)
        {
            return this.NotFound($"Task with ID {id} not found.");
        }

        await this.service.UpdateAsync(task);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existingTask = await this.service.GetByIdAsync(id);
        if (existingTask is null)
        {
            return this.NotFound($"Task with ID {id} not found.");
        }

        await this.service.DeleteByIdAsync(id);
        return this.NoContent();
    }

    [HttpGet("list/{todoListId}")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetByTodoListId(int todoListId)
    {
        var tasks = await this.service.GetByListIdAsync(todoListId);
        if (tasks is null || !tasks.Any())
        {
            return this.NotFound($"No tasks found for Todo List with ID {todoListId}.");
        }

        return this.Ok(tasks);
    }
}
