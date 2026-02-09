using TodoList.Data.Entities;

namespace TodoListApp.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<TaskEntity?> GetByIdAsync(int id);

    Task<TaskEntity?> GetWithDetailsAsync(int id);

    Task<IEnumerable<TaskEntity>> GetByListIdAsync(int todoListId);

    Task<IEnumerable<TaskEntity>> GetAllForUserAsync(string userId);

    Task<bool> ExistsAsync(int id);

    IQueryable<TaskEntity> GetAssignedTasksQuery(string userId);

    IQueryable<TaskEntity> SearchTasksQuery(string userId);

    Task AddAsync(TaskEntity entity);

    Task RemoveAsync(TaskEntity entity);

    Task SaveChangesAsync();
}
