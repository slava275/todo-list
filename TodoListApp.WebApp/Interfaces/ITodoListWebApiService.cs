using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITodoListWebApiService
{
    Task<IEnumerable<TodoListModel>> GetAllAsync();

    Task CreateAsync(TodoListModel model);

    Task UpdateAsync(TodoListModel model);

    Task DeleteByIdAsync(int id);

    Task<TodoListModel> GetByIdAsync(int id);
}
