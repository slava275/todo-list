using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentDatabaseService service;

    public CommentsController(ICommentDatabaseService service)
    {
        this.service = service;
    }

    private string UserId => this.User.GetUserId();

    [HttpGet("tasks/{taskid}")]
    public async Task<ActionResult<IEnumerable<CommentModel>>> GetCommentsByTaskId(int taskid)
    {
        if (taskid <= 0)
        {
            throw new ArgumentException("Невалідний ID завдання.");
        }

        var comments = await this.service.GetByTaskIdAsync(taskid, this.UserId);
        return this.Ok(comments ?? Enumerable.Empty<CommentModel>());
    }

    [HttpPost]
    public async Task<ActionResult> AddComment([FromBody] CommentModel model)
    {
        if (model == null)
        {
            throw new ArgumentException("Модель коментаря порожня.");
        }

        await this.service.AddAsync(model, this.UserId);
        return this.Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateComment([FromBody] CommentModel model)
    {
        if (model == null)
        {
            throw new ArgumentException("Модель коментаря порожня.");
        }

        await this.service.UpdateAsync(model, this.UserId);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Невалідний ID коментаря.");
        }

        await this.service.DeleteAsync(id, this.UserId);
        return this.NoContent();
    }
}
