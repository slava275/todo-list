using Microsoft.AspNetCore.Mvc;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentDatabaseService service;

    public CommentsController(ICommentDatabaseService service)
    {
        this.service = service;
    }

    [HttpGet("tasks/{taskid}")]
    public async Task<ActionResult<IEnumerable<CommentModel>>> GetCommentsByTaskId(int taskid)
    {
        if (taskid <= 0)
        {
            return this.BadRequest("TaskId must be greater than zero.");
        }

        var comments = await this.service.GetByTaskIdAsync(taskid);

        return this.Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult> AddComment([FromBody] CommentModel model)
    {
        if (model == null)
        {
            return this.BadRequest("Comment model cannot be null.");
        }

        await this.service.AddAsync(model);
        return this.Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateComment([FromBody] CommentModel model)
    {
        if (model == null)
        {
            return this.BadRequest("Comment model cannot be null.");
        }

        await this.service.UpdateAsync(model);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        if (id <= 0)
        {
            return this.BadRequest("Id must be greater than zero.");
        }

        await this.service.DeleteAsync(id);
        return this.NoContent();
    }
}
