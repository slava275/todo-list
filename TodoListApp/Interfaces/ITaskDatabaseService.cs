using TodoListApp.Data.Models;

namespace TodoListApp.Interfaces;

public interface ITaskDatabaseService
{
    Task<IEnumerable<TaskModel>> GetByListIdAsync(int todoListId);

    Task<IEnumerable<TaskModel>> GetAllAsync();

    Task<TaskModel?> GetByIdAsync(int id);

    Task CreateAsync(TaskModel item);

    Task DeleteAsync(TaskModel taskModel);

    Task DeleteByIdAsync(int id);

    Task UpdateAsync(TaskModel item);
}
