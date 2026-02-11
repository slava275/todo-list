using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;

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

    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMember(int id, [FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("ID користувача не може бути порожнім.");
        }

        await this.service.AddMemberAsync(id, userId, this.UserId);
        return this.Ok();
    }

    [HttpDelete("{id}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(int id, string memberId)
    {
        await this.service.RemoveMemberAsync(id, memberId, this.UserId);
        return this.NoContent();
    }

    [HttpPatch("{id}/members/{memberId}/role")]
    public async Task<IActionResult> UpdateMemberRole(int id, string memberId, [FromQuery] TodoListRole newRole)
    {
        await this.service.UpdateMemberRoleAsync(id, memberId, newRole, this.UserId);
        return this.NoContent();
    }
}
