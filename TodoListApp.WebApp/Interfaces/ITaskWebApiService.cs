using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITaskWebApiService
{
    Task CreateAsync(TaskModel model);

    Task UpdateAsync(TaskModel model);

    Task DeleteByIdAsync(int id);

    Task<IEnumerable<TaskModel>> GetAllByListIdAsync(int listId);

    Task<TaskModel> GetByIdAsync(int id);
}
