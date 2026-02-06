using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Exceptions;
using TodoListApp.Extensions;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Controllers;

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
            return this.BadRequest();
        }

        var comments = await this.service.GetByTaskIdAsync(taskid, this.UserId);

        return this.Ok(comments ?? Enumerable.Empty<CommentModel>());
    }

    [HttpPost]
    public async Task<ActionResult> AddComment([FromBody] CommentModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        try
        {
            await this.service.AddAsync(model, this.UserId);
            return this.Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (AccessDeniedException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateComment([FromBody] CommentModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        try
        {
            await this.service.UpdateAsync(model, this.UserId);
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
        catch (ArgumentNullException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        if (id <= 0)
        {
            return this.BadRequest();
        }

        try
        {
            await this.service.DeleteAsync(id, this.UserId);
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
        catch (ArgumentOutOfRangeException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }
}
