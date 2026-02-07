using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Extensions;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoListsController : ControllerBase
{
    private readonly ITodoListDatabaseService service;

    public TodoListsController(ITodoListDatabaseService service)
    {
        this.service = service;
    }

    private string UserId => this.User.GetUserId();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListModel>>> GetAll()
    {
        var todoLists = await this.service.GetAllAsync(this.UserId);
        return this.Ok(todoLists ?? Enumerable.Empty<TodoListModel>());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListModel>> GetById(int id)
    {
        var todoList = await this.service.GetByIdAsync(id, this.UserId);
        return this.Ok(todoList);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] TodoListModel todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        await this.service.CreateAsync(todoList, this.UserId);

        return this.CreatedAtAction(nameof(this.GetById), new { id = todoList.Id }, todoList);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] TodoListModel todoList)
    {
        if (todoList is null || todoList.Id != id)
        {
            throw new ArgumentException("The ID in the URL does not match the ID in the request body.");
        }

        await this.service.UpdateAsync(todoList, this.UserId);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await this.service.DeleteByIdAsync(id, this.UserId);
        return this.NoContent();
    }
}
