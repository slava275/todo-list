using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITaskWebApiService
{
    Task CreateAsync(TaskModel model);

    Task UpdateAsync(TaskModel model);

    Task DeleteByIdAsync(int id);

    Task<IEnumerable<TaskModel>> GetAllByListIdAsync(int listId);

    Task<TaskModel> GetByIdAsync(int id);

    Task<IEnumerable<TaskModel>> GetAllByUserIdAsync(Statuses? status = null, string sortBy = "name", bool isAscending = true);

    Task<IEnumerable<TaskModel>> SearchAsync(string? title, DateTime? dueDate, DateTime? createdAt);

    Task AssignTaskAsync(int taskId, string userId);
}
