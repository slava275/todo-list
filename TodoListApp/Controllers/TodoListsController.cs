using Microsoft.AspNetCore.Mvc;
using TodoListApp.Data.Models;
using TodoListApp.Interfaces;

namespace TodoListApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoListsController : ControllerBase
{
    private readonly ITodoListDatabaseService service;

    public TodoListsController(ITodoListDatabaseService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult<TodoListModel>> GetAll()
    {
        var todoLists = await this.service.GetAllAsync();

        if (todoLists is null || !todoLists.Any())
        {
            return this.NotFound("No todo lists found.");
        }

        return this.Ok(todoLists);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListModel>> GetById(int id)
    {
        var todoList = await this.service.GetByIdAsync(id);
        if (todoList is null)
        {
            return this.NotFound($"Todo list with ID {id} not found.");
        }

        return this.Ok(todoList);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TodoListModel todoList)
    {
        if (todoList is null)
        {
            return this.BadRequest("Todo list cannot be null.");
        }

        await this.service.CreateAsync(todoList);
        return this.CreatedAtAction(nameof(this.GetById), new { id = todoList.Id }, todoList);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] TodoListModel todoList)
    {
        if (todoList is null || todoList.Id != id)
        {
            return this.BadRequest("Invalid todo list data.");
        }

        var existingTodoList = await this.service.GetByIdAsync(id);
        if (existingTodoList is null)
        {
            return this.NotFound($"Todo list with ID {id} not found.");
        }

        await this.service.UpdateAsync(todoList);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existingTodoList = await this.service.GetByIdAsync(id);
        if (existingTodoList is null)
        {
            return this.NotFound($"Todo list with ID {id} not found.");
        }

        await this.service.DeleteByIdAsync(id);
        return this.NoContent();
    }
}
