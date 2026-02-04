using Microsoft.AspNetCore.Mvc;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagDatabaseService tagService;

    public TagsController(ITagDatabaseService tagService)
    {
        this.tagService = tagService;
    }

    // US18: Отримати всі мої теги
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetAll()
    {
        try
        {
            int userId = 0;
            var tags = await this.tagService.GetAllTagsAsync(userId);
            return this.Ok(tags);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    // US20: Додати тег до завдання
    [HttpPost("{taskId}")]
    public async Task<IActionResult> AddTag(int taskId, [FromBody] TagModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Name))
        {
            return this.BadRequest("Tag name is required.");
        }

        try
        {
            int userId = 0;
            await this.tagService.AddTagToTaskAsync(taskId, model, userId);
            return this.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    // US19: Завдання за тегом
    [HttpGet("{tagId}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasksByTag(int tagId)
    {
        try
        {
            int userId = 0;
            var tasks = await this.tagService.GetTasksByTagIdAsync(tagId, userId);
            return this.Ok(tasks);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    // US21: Видалити тег з завдання
    [HttpDelete("{taskId}/{tagId}")]
    public async Task<IActionResult> RemoveTag(int taskId, int tagId)
    {
        try
        {
            int userId = 0;
            await this.tagService.RemoveTagFromTaskAsync(taskId, tagId, userId);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return this.BadRequest(ex.Message);
        }
    }
}
