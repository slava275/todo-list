using TodoListShared.Models.Models;

namespace TodoListApp.Interfaces;

public interface ITagDatabaseService
{
    Task<IEnumerable<TagModel>> GetAllTagsAsync(int userId);

    Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId, int userId);

    Task AddTagToTaskAsync(int taskId, TagModel model, int userId);

    Task RemoveTagFromTaskAsync(int taskId, int tagId, int userId);
}
