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
public class TagsController : ControllerBase
{
    private readonly ITagDatabaseService tagService;

    public TagsController(ITagDatabaseService tagService)
    {
        this.tagService = tagService;
    }

    private string UserId => this.User.GetUserId();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetAll()
    {
        var tags = await this.tagService.GetAllTagsAsync(this.UserId);
        return this.Ok(tags ?? Enumerable.Empty<TagModel>());
    }

    [HttpPost("{taskId}")]
    public async Task<IActionResult> AddTag(int taskId, [FromBody] TagModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Name))
        {
            return this.BadRequest();
        }

        try
        {
            await this.tagService.AddTagToTaskAsync(taskId, model, this.UserId);
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
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpGet("{tagId}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasksByTag(int tagId)
    {
        try
        {
            var tasks = await this.tagService.GetTasksByTagIdAsync(tagId, this.UserId);
            return this.Ok(tasks ?? Enumerable.Empty<TaskModel>());
        }
        catch (EntityNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpDelete("{taskId}/{tagId}")]
    public async Task<IActionResult> RemoveTag(int taskId, int tagId)
    {
        try
        {
            await this.tagService.RemoveTagFromTaskAsync(taskId, tagId, this.UserId);
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
}
