using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.Database.Interfaces;

public interface ITagDatabaseService
{
    Task<IEnumerable<TagModel>> GetAllTagsAsync(string userId);

    Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId, string userId);

    Task AddTagToTaskAsync(int taskId, TagModel model, string userId);

    Task RemoveTagFromTaskAsync(int taskId, int tagId, string userId);
}
