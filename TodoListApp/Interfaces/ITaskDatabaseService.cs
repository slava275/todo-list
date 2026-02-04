using TodoListShared.Models;
using TodoListShared.Models.Models;

namespace TodoListApp.Interfaces;

public interface ITaskDatabaseService
{
    Task<IEnumerable<TaskModel>> GetByListIdAsync(int todoListId);

    Task<IEnumerable<TaskModel>> GetAssignedTasksAsync(int userId, Statuses? status, string sortBy, bool ascending);

    Task ChangeStatusAsync(int id, Statuses status);

    Task<IEnumerable<TaskModel>> GetAllAsync();

    Task<TaskModel?> GetByIdAsync(int id);

    Task CreateAsync(TaskModel item);

    Task DeleteAsync(TaskModel taskModel);

    Task DeleteByIdAsync(int id);

    Task UpdateAsync(TaskModel item);

    Task<IEnumerable<TaskModel>> SerchTasksAsync(string? title, DateTime? dueDate, DateTime? createdAt);
}
