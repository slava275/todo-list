using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Controllers;

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
            throw new ArgumentException("Назва тегу не може бути порожньою.");
        }

        await this.tagService.AddTagToTaskAsync(taskId, model, this.UserId);
        return this.Ok();
    }

    [HttpGet("{tagId}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasksByTag(int tagId)
    {
        var tasks = await this.tagService.GetTasksByTagIdAsync(tagId, this.UserId);
        return this.Ok(tasks ?? Enumerable.Empty<TaskModel>());
    }

    [HttpDelete("{taskId}/{tagId}")]
    public async Task<IActionResult> RemoveTag(int taskId, int tagId)
    {
        await this.tagService.RemoveTagFromTaskAsync(taskId, tagId, this.UserId);
        return this.NoContent();
    }
}
