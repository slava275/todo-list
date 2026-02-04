using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITagWebApiService
{
    // GET: api/tags
    // US18: Отримати всі теги поточного користувача
    Task<IEnumerable<TagModel>> GetAllTagsAsync();

    // POST: api/tags/{taskId}
    // US20: Додати тег до конкретного завдання
    Task AddTagToTaskAsync(int taskId, TagModel model);

    // GET: api/tags/{tagId}/tasks
    // US19: Отримати список завдань, що мають цей тег
    Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId);

    // DELETE: api/tags/{taskId}/{tagId}
    // US21: Видалити зв'язок між завданням та тегом
    Task RemoveTagFromTaskAsync(int taskId, int tagId);
}
